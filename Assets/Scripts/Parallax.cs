using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Camera cam;
    GameObject[,] bgs;
    public bool horizontal = true;
    public bool vertical = true;
    public Vector2 parallaxAmount = Vector2.zero;

    Vector2 camStartPosition = Vector2.zero;
    Vector2 startPosition = Vector2.zero;
    Vector2 spriteSize;
    private void Awake()
    {
        Sprite spr = spriteRenderer.sprite;
        spriteSize = spr.bounds.size;

        bgs = new GameObject[horizontal ? 3 : 1, vertical ? 3 : 1];
        int[] horiz_coords = horizontal ? new int[] { 0, 1, 2 } : new int[] { 0 };
        int[] vert_coords = vertical ? new int[] { 0, 1, 2 } : new int[] { 0 };
        int horiz_center = horizontal ? 1 : 0;
        int vert_center = vertical ? 1 : 0;
        Vector2 center = new Vector2(horiz_center, vert_center);
        foreach (int x in horiz_coords)
        {
            foreach (int y in vert_coords)
            {
                bgs[x, y] = new GameObject();
                SpriteRenderer renderer = bgs[x, y].AddComponent<SpriteRenderer>();

                renderer.sprite = spr;
                renderer.sortingOrder = spriteRenderer.sortingOrder;
                renderer.sortingLayerID = spriteRenderer.sortingLayerID;
                renderer.color = spriteRenderer.color;

                Vector2 pos = center - new Vector2(x, y);
                pos.Scale(spriteSize);
                bgs[x, y].transform.parent = transform;
                bgs[x, y].transform.localPosition = pos;
            }
        }
        Destroy(spriteRenderer);

        camStartPosition = cam.transform.position;
        startPosition = transform.position;
    }

    private void LateUpdate()
    {
        Vector2 camPosition = cam.transform.position;
        Vector2 camDifference = camPosition - camStartPosition;

        Vector2 newPosition = startPosition + camDifference * parallaxAmount;

        Vector2 normalizedOffset = newPosition - camPosition;
        normalizedOffset.Scale(new Vector2(1 / spriteSize.x, 1 / spriteSize.y));

        Vector2 integerNormalizedOffset = new Vector2(Mathf.Round(normalizedOffset.x), Mathf.Round(normalizedOffset.y));
        newPosition -= spriteSize * integerNormalizedOffset;

        transform.position = newPosition;
    }
}
