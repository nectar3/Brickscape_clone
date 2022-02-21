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

    private Rigidbody rb;

    bool isDragging = false;

    private GameObject rayPoint_max;
    private GameObject rayPoint_min;


    private void Start()
    {
        spaceSize = GameManager.I.spaceSize;

        blockSize =
            dir == Direction.x ? Mathf.RoundToInt(transform.localScale.x) :
            dir == Direction.y ? Mathf.RoundToInt(transform.localScale.y) :
            dir == Direction.z ? Mathf.RoundToInt(transform.localScale.z) : 1;

         d = transform.position;
        //Debug.Log(spaceSize);
        //Debug.Log(blockSize);
        limit = (spaceSize - blockSize) / (float)2;
        //Debug.Log(limit);

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
        isDragging = true;
    }

    // TODO: 드래그 빨리하면 뚫어버리는 문제
    Vector3 d;
    public void OnDrag(PointerEventData eventData)
    {
        if(dir == Direction.y)
        {
            float deltaY = eventData.delta.y;
            if(deltaY > 5)
                Debug.Log("쎄게박음 = ");
            deltaY = Mathf.Clamp(deltaY, -2, 2);
            d = Vector3.up * deltaY * dragSpeed * Time.deltaTime;
            if (Physics.Raycast(rayPoint_max.transform.position, Vector3.up, out RaycastHit hit, 1f))
            {
                if(deltaY > 0 && hit.distance < 0.1f)
                    d = Vector3.zero;
            }
            if(Physics.Raycast(rayPoint_min.transform.position, Vector3.down, out RaycastHit hit2, 1f))
            {
                if (deltaY < 0 && hit.distance < 0.1f)
                    d = Vector3.zero;
            }
        }
        var newPos = transform.position + d;
        newPos.y = Mathf.Clamp(newPos.y, -limit, limit); // 델타에 제한을 걸어줘야 한번에 넘어서지 않음
        transform.position = newPos;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

}
