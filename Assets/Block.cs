using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Block : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum Direction
    {
        x = 0, y, z,
    }
    public Direction dir;

    public float dragSpeed = 2f;

    private float spaceSize;
    private int blockSize;
    float limit;

    bool isDragging = false;

    private GameObject rayPoint_max;
    private GameObject rayPoint_min;

    Vector3 d; // 이동할 델타

    private void Start()
    {
        blockSize =
            dir == Direction.x ? Mathf.RoundToInt(transform.localScale.x) :
            dir == Direction.y ? Mathf.RoundToInt(transform.localScale.y) :
            dir == Direction.z ? Mathf.RoundToInt(transform.localScale.z) : 1;

        d = transform.position;

        if (dir == Direction.y)
        {
            var rayPoint = new GameObject("rayPoint_max");
            rayPoint.transform.SetParent(this.transform);
            rayPoint.transform.localPosition = new Vector3(0, (blockSize / 2f) / (float)blockSize, 0); // 블럭의 맨 윗부분
            rayPoint_max = rayPoint;

            rayPoint = new GameObject("rayPoint_min");
            rayPoint.transform.SetParent(this.transform);
            rayPoint.transform.localPosition = new Vector3(0, -(blockSize / 2f) / (float)blockSize, 0);
            rayPoint_min = rayPoint;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag = ");
        isDragging = true;
    }

    // TODO: 드래그 빨리하면 뚫어버려서 delta를 너무 줄이니까 감도가 별로인 문제
    public void OnDrag(PointerEventData eventData)
    {
        if(dir == Direction.y)
        {
            float deltaY = eventData.delta.y;
            deltaY = Mathf.Clamp(deltaY, -2, 2);
            d = Vector3.up * deltaY * dragSpeed * Time.deltaTime;
            if (Physics.Raycast(rayPoint_max.transform.position, Vector3.up, out RaycastHit hit, 1f))
            {
                Debug.Log("hit.distance = " + hit.distance);

                if (deltaY > 0 && hit.distance < 0.1f)
                    d = Vector3.zero;
            }
            if(Physics.Raycast(rayPoint_min.transform.position, Vector3.down, out hit, 1f))
            {
                if (deltaY < 0 && hit.distance < 0.1f)
                {
                    d = Vector3.zero;
                    Debug.Log("hit.distance = " + hit.distance);
                }
            }
        }
        var newPos = transform.position + d;
        transform.position = newPos;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

}
