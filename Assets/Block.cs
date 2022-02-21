using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Block : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public enum Direction
    {
        x = 0, y, z,
    }
    public Direction dir;
    private Vector3 dirVec;

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
        dirVec = 
            dir == Direction.x ? Vector3.right :
            dir == Direction.y ? Vector3.up :
            dir == Direction.z ? Vector3.forward : Vector3.zero;

        blockSize = Mathf.RoundToInt(Vector3.Dot(dirVec, transform.localScale));

        d = Vector3.zero;

        // 블럭 끝부분에 레이캐스트 포인트
        var rayPoint = new GameObject("rayPoint_max");
        rayPoint.transform.SetParent(this.transform);
        rayPoint.transform.localPosition = dirVec /2; // 블럭의 맨 윗부분. (blockSize / 2f) / (float)blockSize 이거 정리하면 항상 1/2 이다
        rayPoint_max = rayPoint;

        rayPoint = new GameObject("rayPoint_min");
        rayPoint.transform.SetParent(this.transform);
        rayPoint.transform.localPosition = -dirVec / 2;
        rayPoint_min = rayPoint;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag = ");
        isDragging = true;
    }


    bool isShaking = false;
    public void ShakeBlock(Vector3 hitPoint, float hitDelta)
    {
        if (isShaking)
            return;
        Debug.Log("hitPoint = " + hitPoint);

        isShaking = true;

        if (Vector3.Dot( transform.position, dirVec) < Vector3.Dot( hitPoint, dirVec)) // 블럭 방향의 성분만 가져오기
        {
            Debug.Log("방향 true");
            transform.DORotate(dirVec * 20, 1f);
        }
        else
        {
            Debug.Log("방향 false");
            transform.DORotate(dirVec * -20, 1f);

        }
    }

    // TODO: 상대방에게 맞은 hitPoint 에 따라 회전 축 정하기
    //Vector3 GetRotationAxisByDir()
    //{
    //    var delta =
    //        dir == Direction.x ? -eventData.delta.x :
    //        dir == Direction.y ? eventData.delta.y :
    //        dir == Direction.z ? Vector3.right : Vector3.right;
    //}

    // TODO: 드래그 빨리하면 뚫어버려서 delta를 너무 줄이니까 감도가 별로인 문제
    public void OnDrag(PointerEventData eventData)
    {
        var delta =
            dir == Direction.x ? -eventData.delta.x :
            dir == Direction.y ? eventData.delta.y :
            dir == Direction.z ? eventData.delta.x : 0;
        var delta_clamped = Mathf.Clamp(delta, -3, 3);

        d = dirVec * delta_clamped * dragSpeed * Time.deltaTime;
        if (Physics.Raycast(rayPoint_max.transform.position, dirVec, out RaycastHit hit, 1f))
        {
            if (delta_clamped > 0 && hit.distance < 0.1f)
            {
                d = Vector3.zero;
                var otherBlock = hit.collider.gameObject.GetComponent<Block>();
                if (otherBlock)
                    otherBlock.ShakeBlock(hit.point, delta);
            }
        }
        if(Physics.Raycast(rayPoint_min.transform.position, -dirVec, out hit, 1f))
        {
            if (delta_clamped < 0 && hit.distance < 0.1f)
            {
                d = Vector3.zero;
                var otherBlock = hit.collider.gameObject.GetComponent<Block>();
                if (otherBlock)
                    otherBlock.ShakeBlock(hit.point, delta);
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
