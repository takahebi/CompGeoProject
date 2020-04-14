using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class V2 : MonoBehaviour
{
    private Vector2Int imageDim;         //Dimension of image as public variable defined in game object in inspector X Y
    private int regionAmount = 0;            //number of sites
	public bool drawByDistance = false; //determine mode
	private float x_test;
	private float y_test;
	private float x_current;
	private float y_current;
    public int dim;
	public List<Vector2Int> vertices;


	private void Start()                //called once at the start of program
	{
		imageDim.x = dim;
		imageDim.y = dim;

		//sprite create method 
		//var spr = GetComponent<SpriteRenderer>().sprite = Sprite.Create((drawByDistance ? GetDiagramByDistance()    // texture: which design to use draw by distance or by distance
		//	: GetDiagram()), new Rect(0, 0, imageDim.x, imageDim.y),                                      // Rect is a unity methon for rectangles  (x, y, width, height) --> width and height defined in game object
		//	Vector2.one * 0.5f);                                                                          // pivot determines what becomes the center of the Sprite  

        var spr = GetComponent<SpriteRenderer>().sprite = Sprite.Create(blackTex(), new Rect(0, 0, imageDim.x, imageDim.y), // Rect is a unity methon for rectangles  (x, y, width, height) --> width and height defined in game object
			Vector2.one * 0.5f);                                    

		//Debug.Log(spr.bounds.extents.x);
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

            //clickPosition.x < 0 ? (clickPosition.x + x_test) 

            x_current = clickPosition.x + x_test/2;
			y_current = y_test/2 - clickPosition.y;
			float x_new = dim / x_test * x_current;
			float y_new = dim / y_test * y_current;

	
			Debug.Log(x_new + " " + y_new);

			vertices.Add(new Vector2Int((int)(x_new), (int)(y_new)));
			GetComponent<SpriteRenderer>().sprite = Sprite.Create(GetDiagram(), new Rect(0, 0, imageDim.x, imageDim.y),Vector2.one * 0.5f);

		}

	}


    //GetDiagram
    Texture2D GetDiagram()
	{
		//Vector2Int[] centroids = new Vector2Int[regionAmount];                                              // centroids = site locations
		Color[] regions = new Color[regionAmount];                                                          // array of colors for color representation  

        for (int i = 0; i < regionAmount; i++)                                                              // for the number of sites defined in public variable
		{
			//centroids[i] = new Vector2Int(Random.Range(0, imageDim.x), Random.Range(0, imageDim.y));        // site location generated randomly from width and height
			//Debug.Log(centroids[i]);
			regions[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);   // random colors generater, number of colors = num regions or sites
		}


		Color[] pixelColors = new Color[imageDim.x * imageDim.y];                                           // pixels is defined by dimensions
		for (int x = 0; x < imageDim.x; x++)                                                                // for width      
		{
			for (int y = 0; y < imageDim.y; y++)                                                            //   for height
			{
				int index = x * imageDim.x + y;                                                             //     index determined by position 
				//pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)];
				pixelColors[index] = regions[GetClosestCentroidIndex(new Vector2Int(x, y))];
			}
		}
		return GetImageFromColorArray(pixelColors);
	}






	//Texture2D GetDiagramByDistance()
	//{
	//	Vector2Int[] centroids = new Vector2Int[regionAmount];

	//	for (int i = 0; i < regionAmount; i++)
	//	{
	//		centroids[i] = new Vector2Int(Random.Range(0, imageDim.x), Random.Range(0, imageDim.y));
	//	}
	//	Color[] pixelColors = new Color[imageDim.x * imageDim.y];
	//	float[] distances = new float[imageDim.x * imageDim.y];

	//	//you can get the max distance in the same pass as you calculate the distances. :P oops!
	//	float maxDst = float.MinValue;
	//	for (int x = 0; x < imageDim.x; x++)
	//	{
	//		for (int y = 0; y < imageDim.y; y++)
	//		{
	//			int index = x * imageDim.x + y;
	//			distances[index] = Vector2.Distance(new Vector2Int(x, y), centroids[GetClosestCentroidIndex(new Vector2Int(x, y), centroids)]);
	//			if (distances[index] > maxDst)
	//			{
	//				maxDst = distances[index];
	//			}
	//		}
	//	}

	//	for (int i = 0; i < distances.Length; i++)
	//	{
	//		float colorValue = distances[i] / maxDst;
	//		pixelColors[i] = new Color(colorValue, colorValue, colorValue, 1f);
	//	}
	//	return GetImageFromColorArray(pixelColors);
	//}
	/* didn't actually need this
	float GetMaxDistance(float[] distances)
	{
		float maxDst = float.MinValue;
		for(int i = 0; i < distances.Length; i++)
		{
			if(distances[i] > maxDst)
			{
				maxDst = distances[i];
			}
		}
		return maxDst;
	}*/




	//int GetClosestCentroidIndex(Vector2Int pixelPos, Vector2Int[] centroids)
	int GetClosestCentroidIndex(Vector2Int pixelPos)
	{
		float smallestDst = float.MaxValue;
		int index = 0;
		//for (int i = 0; i < centroids.Length; i++)
		for (int i = 0; i < vertices.Count; i++)
		{
			//if (Vector2.Distance(pixelPos, centroids[i]) < smallestDst)
			if (Vector2.Distance(pixelPos, vertices[i]) < smallestDst)
			{
				//smallestDst = Vector2.Distance(pixelPos, centroids[i]);
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
