/**
 * @see https://blog.thomasjungblut.com/graph/mincut/mincut/
 */
type Vertex = string;
type Weight = number;
class Edge {
  constructor(readonly source: Vertex, readonly destination: Vertex, readonly weight: Weight) {}
}

class Graph {
  private edgeMap = new Map<Vertex, Map<Vertex, Edge>>();
  private edgeSet: Set<Edge> = new Set();

  copy() {
    const graph = new Graph();
    graph.edgeMap = new Map(Array.from(this.edgeMap, ([key, map]) => [key, new Map(map)]));
    graph.edgeSet = new Set(this.edgeSet);
    return graph;
  }

  setWeight(source: Vertex, destination: Vertex, weight: Weight) {
    const edge = new Edge(source, destination, weight);
    this.edgeSet.add(edge);
    this.addEdge(source, destination, edge);
    this.addEdge(destination, source, edge);
  }

  addWeight(left: Vertex, right: Vertex, weight: Weight) {
    const prev = this.edgeMap.get(left)?.get(right)?.weight ?? 0;
    this.setWeight(left, right, prev + weight);
  }

  private addEdge(source: Vertex, destination: Vertex, edge: Edge) {
    this.edgeMap.set(source, this.edgeMap.get(source) ?? new Map());
    this.edgeMap.get(source)!.set(destination, edge);
  }

  getEdgeMap(source: Vertex) {
    return this.edgeMap.get(source)!;
  }

  getAllEdges() {
    return [...this.edgeSet];
  }

  hasEdge(edge: Edge) {
    return this.edgeSet.has(edge);
  }

  deleteVertex(source: Vertex) {
    const edgeMap = this.getEdgeMap(source);
    this.edgeMap.delete(source);
    for (const edge of edgeMap.values()) {
      this.edgeMap.get(source === edge.destination ? edge.source : edge.destination)!.delete(source);
      this.edgeSet.delete(edge);
    }
  }

  getVertexSet() {
    return new Set(this.edgeMap.keys());
  }

  getNumVertices() {
    return this.edgeMap.size;
  }

  getWeight(source: Vertex, destination: Vertex) {
    return this.edgeMap.get(source)?.get(destination)?.weight ?? 0;
  }
}

interface CutOfThePhase {
  s: Vertex;
  t: Vertex;
  weight: Weight;
}

function maximumAdjacencySearch(graph: Graph): CutOfThePhase {
  const start = graph.getVertexSet().values().next().value;
  const founds: string[] = [start];
  const cutWeights: number[] = [];
  const candidateSet = new Set(graph.getVertexSet());
  candidateSet.delete(start);

  while (candidateSet.size !== 0) {
    let maxNextVertex = '';
    let maxWeight = -1;
    for (const next of candidateSet) {
      let weightSum = 0;
      for (const s of founds) {
        const edge = graph.getWeight(next, s);
        weightSum += edge;
      }

      if (weightSum > maxWeight) {
        maxNextVertex = next;
        maxWeight = weightSum;
      }
    }

    candidateSet.delete(maxNextVertex);
    founds.push(maxNextVertex);
    cutWeights.push(maxWeight);
  }

  const n = founds.length;
  return { s: founds[n - 2], t: founds[n - 1], weight: cutWeights[cutWeights.length - 1] };
}

function computeMinCut(originalGraph: Graph) {
  const graph = originalGraph.copy();
  const currentPartition = new Set<Vertex>();
  let currentBestPartition = new Set<Vertex>();
  let currentBestCut: CutOfThePhase | null = null;
  while (graph.getNumVertices() > 1) {
    const cutOfThePhase = maximumAdjacencySearch(graph);
    currentPartition.add(cutOfThePhase.t);
    if (!currentBestCut || cutOfThePhase.weight < currentBestCut.weight) {
      currentBestCut = cutOfThePhase;
      currentBestPartition = new Set(currentPartition);
    }

    mergeVerticesFromCut(graph, cutOfThePhase);
  }

  return constructMinCutResult(originalGraph, currentBestPartition);
}

function mergeVerticesFromCut(graph: Graph, { t, s }: CutOfThePhase) {
  const edgeMap = graph.getEdgeMap(t);
  graph.deleteVertex(t);
  for (const [destination, edge] of edgeMap) {
    if (destination === s) {
      continue;
    }
    // merge weights into s
    graph.addWeight(destination, s, edge.weight);
  }
}

function constructMinCutResult(graph: Graph, partition: Set<Vertex>) {
  const first = graph.copy();
  const second = graph.copy();
  for (const v of graph.getVertexSet()) {
    if (partition.has(v)) {
      first.deleteVertex(v);
    } else {
      second.deleteVertex(v);
    }
  }

  const cuttingEdges: Edge[] = graph.getAllEdges().filter((edge) => !first.hasEdge(edge) && !second.hasEdge(edge));
  const cutWeight = cuttingEdges.reduce((acc, edge) => acc + edge.weight, 0);
  return { first, second, cuttingEdges, cutWeight };
}

const data = [
  { left: '1', right: '2', weight: 2 },
  { left: '1', right: '5', weight: 3 },

  { left: '2', right: '5', weight: 2 },
  { left: '2', right: '6', weight: 2 },
  { left: '2', right: '3', weight: 3 },

  { left: '3', right: '4', weight: 4 },
  { left: '3', right: '7', weight: 2 },

  { left: '4', right: '7', weight: 2 },
  { left: '4', right: '8', weight: 2 },

  { left: '5', right: '6', weight: 3 },

  { left: '6', right: '7', weight: 1 },
  { left: '7', right: '8', weight: 3 },
];

const graph = new Graph();
for (const d of data) {
  graph.setWeight(d.left, d.right, d.weight);
}

console.log(computeMinCut(graph));

