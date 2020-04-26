using UnityEngine;
using System.Collections.Generic;

using csDelaunay;

public class VoronoiDiagram : MonoBehaviour
{

    public Gradient gradient;
    // The number of polygons/sites we want
    public int polygonNumber = 15;

    // This is where we will store the resulting data
    private Dictionary<Vector2f, Site> sites;
    private List<Edge> edges;

    void Start()
    {
        // Create your sites (lets call that the center of your polygons)
        List<Vector2f> points = CreateRandomPoint();

        // Create the bounds of the voronoi diagram
        // Use Rectf instead of Rect; it's a struct just like Rect and does pretty much the same,
        // but like that it allows you to run the delaunay library outside of unity (which mean also in another tread)
        Rectf bounds = new Rectf(0, 0, 512, 512);

        // There is a two ways you can create the voronoi diagram: with or without the lloyd relaxation
        // Here I used it with 2 iterations of the lloyd relaxation
        Voronoi voronoi = new Voronoi(points, bounds, 5);

        // But you could also create it without lloyd relaxtion and call that function later if you want
        //Voronoi voronoi = new Voronoi(points,bounds);
        //voronoi.LloydRelaxation(5);

        // Now retreive the edges from it, and the new sites position if you used lloyd relaxtion
        sites = voronoi.SitesIndexedByLocation;
        edges = voronoi.Edges;
        //DisplayVoronoiDiagram();

        foreach (KeyValuePair<Vector2f, Site> kv in sites)
            kv.Value.Region(bounds);
        DisplayVoronoiDiagram();


        meshifyV();

    }

    private List<Vector2f> CreateRandomPoint()
    {
        // Use Vector2f, instead of Vector2
        // Vector2f is pretty much the same than Vector2, but like you could run Voronoi in another thread
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < polygonNumber; i++)
        {
            points.Add(new Vector2f((int) Random.Range(100, 400), (int) Random.Range(100, 400)));
        }

        return points;
    }

    // Here is a very simple way to display the result using a simple bresenham line algorithm
    // Just attach this script to a quad
    private void DisplayVoronoiDiagram()
    {
        Texture2D tx = new Texture2D(512, 512);
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {
            tx.SetPixel((int)kv.Key.x, (int)kv.Key.y, Color.red);
        }
        foreach (Edge edge in edges)
        {
            // if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null)
            {
                Debug.Log("missing clipped end");
                    continue;

            }
            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
        tx.Apply();

        this.GetComponent<Renderer>().material.mainTexture = tx;
    }

    // Bresenham line algorithm
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        if (x0 == 0 || x0 == 512 || x1 == 0 || x1 == 512 || y0 == 0 || y0 == 512 || y1 == 0 || y1 == 512) {
            //    Debug.Log(x0);
            //    Debug.Log(y0);
            //    Debug.Log(x1);
            //   Debug.Log(y1);
        }


        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    //-----------------------------------------------------------------------------------------------------
    private void meshifyV()
    {
        int size = sites.Count;
       
        List<Vector3> newVerticesV = new List<Vector3>();
        List<Color> vertexColors = new List<Color>();
        List<Vector3> newTrisV = new List<Vector3>();
        size = 0;
        Rectf bounds = new Rectf(0, 0, 512, 512);
        float uvIndex = 0;
        foreach (KeyValuePair<Vector2f, Site> kv in sites)
        {

            float siteX = kv.Key.x;
            float siteY = kv.Key.y;

            Color siteColor = gradient.Evaluate(sites.Count/10);
            ///help flag
            Debug.Log(siteColor);
            Vector3 siteLocation = new Vector3(siteX, siteY, 0);
            newVerticesV.Add(siteLocation);

          //  float tempUV = 0;
            uvIndex++;

            Vector2f firstPoint = Vector2f.left;
            Vector2f lastPoint = Vector2f.left;
            Vector2f previousPoint = Vector2f.left;
            foreach (Vector2f currentPoint in kv.Value.Region(bounds))
            {
                if (previousPoint == Vector2f.left)
                {
                    previousPoint = currentPoint;
                    firstPoint = currentPoint;
                    vertexColors.Add(siteColor);
                    continue;
                }

                addTriangle(newVerticesV, vertexColors, newTrisV, siteX, siteY, siteColor, siteLocation, previousPoint, currentPoint);
                previousPoint = currentPoint;
                lastPoint = currentPoint;
            }
            addTriangle(newVerticesV, vertexColors, newTrisV, siteX, siteY, siteColor, siteLocation, lastPoint, firstPoint);
        }

        int vectsize = newTrisV.Count;
        int[] newTriangles = new int[vectsize];
        int i = 0;
        foreach (Vector3 cur in newTrisV)
        {
            int index = newVerticesV.FindIndex
                (v3 => v3.Equals(cur));
            newTriangles[i] = index;
            i++;
        }


        Vector2[] uvs = new Vector2[newVerticesV.ToArray().Length];
     //   for ( i = 0; i < uvs.Length; i++)
       // {
          //  uvs[i] = gradient.;
      //  }
        Color[] colors = vertexColors.ToArray();
        Debug.Log("new vertices size" + uvs.Length);
        Debug.Log("colors size" + vertexColors.Count);
        Debug.Log("colors size" + colors.Length);
        Debug.Log("new triangles size" + newTriangles.Length);
        foreach (Color color in vertexColors)
        {
            Debug.Log(color);
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = newVerticesV.ToArray();
    //    mesh.uv = uvs;
        mesh.triangles = newTriangles;
        mesh.colors = colors;
        
       
    }

    private static bool addTriangle(List<Vector3> newVerticesV, List<Color> vertexColors, List<Vector3> newTrisV, 
        float siteX, float siteY, 
        Color siteColor, Vector3 siteLocation, 
        Vector2f previousPoint, Vector2f currentPoint)
    {
        float mx = (int)currentPoint.x;
        float my = (int)currentPoint.y;
        Vector3 trueMain = new Vector3((mx), my, 0);
        newVerticesV.Add(trueMain);
        vertexColors.Add(siteColor);
        //help flag
        float nx = (int)previousPoint.x;
        float ny = (int)previousPoint.y;
        Vector3 truePrevious = new Vector3((nx), ny, 0);
        bool orientation =
            (my - siteY) * (nx - mx) -
            (mx - siteX) * (ny - my) > 0;
        if (orientation)
        {
            newTrisV.Add(truePrevious);
            newTrisV.Add(trueMain);
            newTrisV.Add(siteLocation);
        }
        else
        {
            newTrisV.Add(trueMain);
            newTrisV.Add(truePrevious);
            newTrisV.Add(siteLocation);
        }

        return orientation;
    }
    //----------------------------------------------------------------------------------------------------------
}
