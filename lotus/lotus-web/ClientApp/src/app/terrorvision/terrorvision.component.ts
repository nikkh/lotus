import { Component, OnInit, Inject } from '@angular/core';
import * as go from 'gojs';
import { DataSyncService, DiagramComponent, PaletteComponent } from 'gojs-angular';
import {RadialLayout} from '../RadialLayout';
import { Observable, of, throwError } from 'rxjs';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { catchError, tap, map } from 'rxjs/operators';

@Component({
  selector: 'app-terrorvision',
  templateUrl: './terrorvision.component.html',
  styleUrls: ['./terrorvision.component.css']
})
export class TerrorvisionComponent implements OnInit {
  public network: LotusNetwork;
  public observableNetwork: Observable<LotusNetwork>;
  public myDiagram: any;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}

  getObservableNetwork(): Observable<LotusNetwork>
  {
    return this.http.get<any>(this.baseUrl + 'lotusnetwork')
      .pipe(
        tap(product => console.log('fetched LotusNetwork')),
        catchError(this.handleError('getObservableNetwork', []))
      );
  }

  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }


  ngOnInit(): void
  {
   
    this.getObservableNetwork()
      .subscribe((res: any) => {
        this.network = res;
        this.initDiagram();
        this.generateGraph();
      }, err => {
        console.log(err);
      });
  }


  public initDiagram() {

    const $ = go.GraphObject.make;
    

     this.myDiagram =
      $(go.Diagram, "myDiagramDiv", // must be the ID or reference to div
        {
          initialAutoScale: go.Diagram.Uniform,
          padding: 10,
          isReadOnly: true,
          layout: $(RadialLayout, {
            maxLayers: 2,
            rotateNode: function(node, angle, sweep, radius) {
              // rotate the nodes and make sure the text is not upside-down
              node.angle = angle;
              var label = node.findObject("TEXTBLOCK");
              if (label !== null) {
                label.angle = ((angle > 90 && angle < 270 || angle < -90) ? 180 : 0);
              }
            },
            commitLayers: function() {
              // optional: add circles in the background
              // need to remove any old ones first
              var diagram = this.diagram;
              var gridlayer = diagram.findLayer("Grid");
              var circles = new go.Set(/*go.Part*/);
              gridlayer.parts.each(function(circle) {
                if (circle.name === "CIRCLE") circles.add(circle);
              });
              circles.each(function(circle) {
                diagram.remove(circle);
              });
              // add circles centered at the root
              var $ = go.GraphObject.make;  // for conciseness in defining templates
              for (var lay = 1; lay <= this.maxLayers; lay++) {
                var radius = lay * this.layerThickness;
                var circle =
                  $(go.Part,
                    { name: "CIRCLE", layerName: "Grid" },
                    { locationSpot: go.Spot.Center, location: this.root.location },
                    $(go.Shape, "Circle",
                      { width: radius * 2, height: radius * 2 },
                      { fill: "rgba(200,200,200,0.2)", stroke: null }));
                diagram.add(circle);
              }
            }
          }),
          "animationManager.isEnabled": true
        });

    // shows when hovering over a node
    var commonToolTip =
      $("ToolTip",
        $(go.Panel, "Vertical",
          { margin: 3 },
          $(go.TextBlock,  // bound to node data
            { margin: 4, font: "bold 12pt sans-serif" },
            new go.Binding("text")),
          $(go.TextBlock,  // bound to node data
            new go.Binding("text", "color", function(c) { return "Color: " + c; })),
          $(go.TextBlock,  // bound to Adornment because of call to Binding.ofObject
            new go.Binding("text", "", function(ad) { return "Connections: " + ad.adornedPart.linksConnected.count; }).ofObject())
        )  // end Vertical Panel
      );  // end Adornment

    // define the Node template
    this.myDiagram.nodeTemplate =
      $(go.Node, "Spot",
        {
          locationSpot: go.Spot.Center,
          locationObjectName: "SHAPE",  // Node.location is the center of the Shape
          selectionAdorned: false,
          click: this.nodeClicked,
          toolTip: commonToolTip
        },
        $(go.Shape, "Circle",
          {
            name: "SHAPE",
            fill: "lightgray",  // default value, but also data-bound
            stroke: "transparent",
            strokeWidth: 2,
            desiredSize: new go.Size(20, 20),
            portId: ""  // so links will go to the shape, not the whole node
          },
          new go.Binding("fill", "color")),
        $(go.TextBlock,
          {
            name: "TEXTBLOCK",
            alignment: go.Spot.Right,
            alignmentFocus: go.Spot.Left
          },
          new go.Binding("text"))
      );

    // this is the root node, at the center of the circular layers
    this.myDiagram.nodeTemplateMap.add("Root",
      $(go.Node, "Auto",
        {
          locationSpot: go.Spot.Center,
          selectionAdorned: false,
          toolTip: commonToolTip
        },
        $(go.Shape, "Circle",
          { fill: "white" }),
        $(go.TextBlock,
          { font: "bold 12pt sans-serif", margin: 5 },
          new go.Binding("text"))
      ));

    // define the Link template
    this.myDiagram.linkTemplate =
      $(go.Link,
        {
          routing: go.Link.Normal,
          curve: go.Link.Bezier,
          selectionAdorned: false,
          layerName: "Background"
        },
        $(go.Shape,
          {
            stroke: "black",  // default value, but is data-bound
            strokeWidth: 1
          },
          new go.Binding("stroke", "color"))
      );
  }

  public  nodeClicked(e, root) {
    var diagram = root.diagram;
    if (diagram === null) return;
    // all other nodes should be visible and use the default category
    diagram.nodes.each(function(n) {
      n.visible = true;
      if (n !== root) n.category = "";
    })
    // make this Node the root
    root.category = "Root";
    // tell the RadialLayout what the root node should be
    diagram.layout.root = root;
    diagram.layoutDiagram(true);
  }
  public generateGraph() {
   
    var nodeDataArray = [];
    for (var i = 0; i < this.network.nodes.length; i++) {
      nodeDataArray.push({ key: this.network.nodes[i].key, text: this.network.nodes[i].text, color: this.network.nodes[i].colour });
    }

    var linkDataArray = [];
    var num = this.network.links.length;
    for (var i = 0; i < num; i++) {
      linkDataArray.push({ from: this.network.links[i].from, to: this.network.links[i].to, color: this.network.links[i].colour });
    }

    this.myDiagram.model = new go.GraphLinksModel(nodeDataArray, linkDataArray);
    var someone = nodeDataArray[Math.floor(Math.random() * nodeDataArray.length)];
    this.nodeClicked(null, this.myDiagram.findNodeForData(someone));
  }

  
  public adjustMaxLayers() {
    var nick = document.getElementById('maxLayersChanger') as HTMLInputElement;
    var newMaxLayers = parseInt(nick.value);
    function isInteger(val) {
      return typeof val === 'number' &&
        isFinite(val) &&
        Math.floor(val) === val;
    }
    if (!isInteger(newMaxLayers) || newMaxLayers < 1 || newMaxLayers > 10) {
      alert("Please enter an integer larger than zero and less than or equal to 10.");
    } else {
      this.myDiagram.layout.maxLayers = Math.max(1, Math.min(newMaxLayers, 10));
      this.nodeClicked(null, this.myDiagram.layout.root);
    }
  }

}

interface LotusNetwork {
  nodes: NodeData[];
  links: LinkData[];
}

interface NodeData {
  key: number;
  text: string;
  colour: string;
}

interface LinkData {
  from: number;
  to: number;
  colour: string;
}

