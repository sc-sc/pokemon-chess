using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
    public const int MaxTouchCount = 5;

    public int maxTouchCount;
    private Touchable[] touchedObjects;
    private Touchable specialTouchObject;

    // Start is called before the first frame update
    void Start()
    {
        touchedObjects = new Touchable[MaxTouchCount];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
            touchDown(hit, 0);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            touchUp(hit.point, 0);
        }
        else if (Input.GetMouseButton(0))
        {
            touchMove(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0);
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            specialTouchObject = hit.collider?.gameObject.GetComponent<Touchable>();

            specialTouchObject?.SpecialTouched(hit.point);
        } else if (Input.GetMouseButtonUp(1))
        {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            specialTouchObject?.SpecialReleased(hit.point);
            specialTouchObject = null;
        }
        
        
        for(int index = 0; index < Input.touchCount; index++)
        {
            Touch touch = Input.GetTouch(index);

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
            
            switch(touch.phase)
            {
                case TouchPhase.Began:
                    touchDown(hit, index);
                    break;
                case TouchPhase.Moved:
                    touchMove(hit.point, index);
                    break;
                default:
                    touchUp(hit.point, index);
                    break;
            }
        }
    }

    private void touchDown(RaycastHit2D hit, int index)
    {
        Touchable touchedObject = hit.collider?.gameObject.GetComponent<Touchable>();
        touchedObjects[index] = touchedObject;

        touchedObject?.Touched(hit.point);
    }

    private void touchMove(Vector2 to, int index)
    {
        touchedObjects[index]?.Moved(to);
    }

    private void touchUp(Vector2 at, int index)
    {
        touchedObjects[index]?.Released(at);
        touchedObjects[index] = null;
    }

    public void Delegate(Touchable from, Touchable to)
    {
        for (int i = 0; i < MaxTouchCount; i++)
        {
            if (touchedObjects[i] == from)
            {
                touchedObjects[i] = to;
                return;
            }
        }
    }
}
