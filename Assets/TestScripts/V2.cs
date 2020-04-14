using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2 : MonoBehaviour
{
    private Vector2Int imageDim;         
    private int regionAmount = 0;            
	public bool drawByDistance = false; 
	private float x_test;
	private float y_test;
	private float x_current;
	private float y_current;
    public int dim;
	public List<Vector2Int> vertices;


	private void Start()                
	{
		imageDim.x = dim;
		imageDim.y = dim;                                                                   

        var spr = GetComponent<SpriteRenderer>().sprite = Sprite.Create(blackTex(), new Rect(0, 0, imageDim.x, imageDim.y), 
			Vector2.one * 0.5f);                                    


		x_test = spr.bounds.extents.x * 2;
		y_test = spr.bounds.extents.y * 2;
	}


    //testing for mouse input to add points
    private void Update()
    {
        //dynamic vertices
		if (Input.GetMouseButtonDown(0))
        {
			Destroy(GetComponent<Sprite>());
			regionAmount++;
			Vector3 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			x_current = clickPosition.y + y_test/2;
			y_current =clickPosition.x + x_test / 2;

			Debug.Log(clickPosition.y + "   " + clickPosition.x);
			Debug.Log(x_current + "   " + y_current);


			float x_new = dim / x_test * x_current;
			float y_new = dim / y_test * y_current;

            //Debug.Log()

			vertices.Add(new Vector2Int((int)(x_new), (int)(y_new)));
			
			GetComponent<SpriteRenderer>().sprite = Sprite.Create((drawByDistance ? GetDiagramByDistance(): GetDiagram()),
                new Rect(0, 0, imageDim.x, imageDim.y),Vector2.one * 0.5f);

		}

	}


    //GetDiagram
    Texture2D GetDiagram()
	{                                             
		Color[] regions = new Color[regionAmount];                                                         

        for (int i = 0; i < regionAmount; i++)                                                             
		{    
			regions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);   
		}


		Color[] pixelColors = new Color[imageDim.x * imageDim.y];                                           
		for (int x = 0; x < imageDim.x; x++)                                                                
		{
			for (int y = 0; y < imageDim.y; y++)                                                          
			{
				int index = x * imageDim.x + y;                                                             
				pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y))];
			}
		}
		return GetImageFromColorArray(pixelColors);
	}


    Texture2D GetDiagramByDistance()
    {
        Color[] pixelColors = new Color[imageDim.x * imageDim.y];
        float[] distances = new float[imageDim.x * imageDim.y];

        float maxDst = float.MinValue;
        for (int x = 0; x < imageDim.x; x++)
        {
            for (int y = 0; y < imageDim.y; y++)
            {
                int index = x * imageDim.x + y;
                distances[index] = Vector2.Distance(new Vector2Int(x, y), vertices[GetClosestCentroidIndex(new Vector2Int(x, y))]);
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
        return GetImageFromColorArray(pixelColors);
    }


    int GetClosestCentroidIndex(Vector2Int pixelPos)
	{
		float smallestDst = float.MaxValue;
		int index = 0;

		for (int i = 0; i < vertices.Count; i++)
		{
			if (Vector2.Distance(pixelPos, vertices[i]) < smallestDst)
			{
				smallestDst = Vector2.Distance(pixelPos, vertices[i]);
				index = i;
			}
		}
		return index;
	}

	Texture2D GetImageFromColorArray(Color[] pixelColors)
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
