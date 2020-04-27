using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class V2 : MonoBehaviour
{
    Vector2Int imageDim;         
    int siteNumber = 0;
    
	private float x_test;
	private float y_test;
	private float x_current;
	private float y_current;
    public int dim;
	List<Vector2Int> vertices;
    public bool manhattan;

	Stopwatch stopwatch;
	string path;

	//intialize 
	private void Start()                
	{
		path = Application.dataPath + "/timingLogA.csv";
		File.WriteAllText(path, "Timing System A Log\n\n");
		File.AppendAllText(path, "Number of Sites, Time (ms)\n");

		imageDim.x = dim;
		imageDim.y = dim;                                                                   

        var spr = GetComponent<SpriteRenderer>().sprite = Sprite.Create(blackTex(), new Rect(0, 0, imageDim.x, imageDim.y), 
			Vector2.one * 0.5f);

		vertices = new List<Vector2Int>();
		x_test = spr.bounds.extents.x * 2;
		y_test = spr.bounds.extents.y * 2;
	}


    //Use Mouse location and translate position to sprite to generate site
    private void Update()
    {
		
        if (Input.GetMouseButtonDown(0))
        {
			stopwatch = Stopwatch.StartNew();
			Destroy(GetComponent<Sprite>());
			siteNumber++;
			Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			x_current = clickPosition.y + y_test/2;
			y_current =clickPosition.x + x_test / 2;

			float x_new = dim / x_test * x_current;
			float y_new = dim / y_test * y_current;

			vertices.Add(new Vector2Int((int)(x_new), (int)(y_new)));
			GetComponent<SpriteRenderer>().sprite = Sprite.Create(VoronoiDiagram(),
                new Rect(0, 0, imageDim.x, imageDim.y),Vector2.one * 0.5f);

            UnityEngine.Debug.Log("Number of Sites: " + siteNumber + " Time to Generate Diagram (ms): " + stopwatch.ElapsedMilliseconds);
			string content = siteNumber + "," + stopwatch.ElapsedMilliseconds + "\n";
			File.AppendAllText(path, content);
		}


	}

    Texture2D VoronoiDiagram()
    {
        Color[] pixelColors = new Color[imageDim.x * imageDim.y];
        float[] distances = new float[imageDim.x * imageDim.y];

        float maxDst = float.MinValue;

        for (int x = 0; x < imageDim.x; x++)
        {
            for (int y = 0; y < imageDim.y; y++)
            {
                int index = x * imageDim.x + y;
                distances[index] = Vector2.Distance(new Vector2Int(x, y), vertices[NearestSite(new Vector2Int(x, y))]);
				if (distances[index] > maxDst)
                {
                    maxDst = distances[index];
                }
            }
        }

        for (int i = 0; i < distances.Length; i++)
        {
            float colorValue = distances[i] / maxDst;
            pixelColors[i] = new Color(colorValue, colorValue, colorValue, 1f);
        }

        return CreateTexure(pixelColors);
    }

    float Manhattan(Vector2 a, Vector2 b)
    {
		return Mathf.Abs(b.x - a.x) + Mathf.Abs(b.y - a.y);
    }


    int NearestSite(Vector2Int pixelPos)
	{
		float smallestDst = float.MaxValue;
		int index = 0;

		for (int i = 0; i < vertices.Count; i++)
		{
			if (manhattan)
			{
				if (Manhattan(pixelPos, vertices[i]) < smallestDst)
				{
					smallestDst = Manhattan(pixelPos, vertices[i]);
					index = i;
				}

			}
			else
			{
				if (Vector2.Distance(pixelPos, vertices[i]) < smallestDst)
				{
					smallestDst = Vector2.Distance(pixelPos, vertices[i]);
					index = i;
				}
			}
		}
		return index;
	}

	Texture2D CreateTexure(Color[] pixelColors)
	{
		Texture2D tex = new Texture2D(imageDim.x, imageDim.y);
		tex.filterMode = FilterMode.Point;
		tex.SetPixels(pixelColors);
		tex.Apply();
		return tex;
	}


    //starting texture
	Texture2D blackTex()
	{
		Color[] pixelColors = new Color[imageDim.x * imageDim.y];
		for (int x = 0; x < imageDim.x; x++)                                                                   
		{
			for (int y = 0; y < imageDim.y; y++)                                                            
			{
				int index = x * imageDim.x + y;                                                             																						
				pixelColors[index] = Color.black;
			}
		}
		Texture2D tex = new Texture2D(imageDim.x, imageDim.y);
		tex.filterMode = FilterMode.Point;
		tex.SetPixels(pixelColors);
		tex.Apply();
		return tex;
	}
}
