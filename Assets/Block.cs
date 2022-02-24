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
    public Direction dir__;

    public LayerMask RayLayer; // 블록을 막아줄 오브젝트들

    private Vector3 dirVec;
    public float dragSpeed = 2f;

    bool isDragging = false;
    bool isDone = false;

    private GameObject rayPoint_max;
    private GameObject rayPoint_min;
    

    private void Start()
    {
        dirVec = 
            dir__ == Direction.x ? Vector3.right :
            dir__ == Direction.y ? Vector3.up :
            dir__ == Direction.z ? Vector3.forward : Vector3.zero;
        //dirVec = transform.InverseTransformDirection(dirVec);

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

    public void ReachExitMove(Vector3 dir)
    {
        GameManager.I.isStageClear = true;
        isDone = true;
        isDragging = false;
        Sequence seq = DOTween.Sequence()
        .Append(transform.DOMove(transform.position + dir.normalized * 5f, 1.5f))
        .OnComplete(() =>
        {
            Debug.Log("클리어 = ");
        });
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDone) return;
        isDragging = true;

        GameManager.I.isBlockDragging = true;
    }

    bool isShaking = false;
    public void ShakeBlock(Vector3 dir, float hitDelta)
    {
        if (isShaking || Mathf.Abs(hitDelta) < 5)
            return;
        isShaking = true;
        Sequence seq = DOTween.Sequence()
            .Append(transform.DOMove(transform.position + dir * 0.1f, 0.1f))
            .Append(transform.DOMove(transform.position, 0.1f))
            .OnComplete(() =>
            {
                isShaking = false;
            });
    }
    Vector3 GetLocalDir()
    {
        return
            dir__ == Direction.x ? transform.right :
            dir__ == Direction.y ? transform.up :
            dir__ == Direction.z ? transform.forward : Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging == false) return;

        var delta =
            dir__ == Direction.x ? -eventData.delta.x :
            dir__ == Direction.y ? eventData.delta.y :
            dir__ == Direction.z ? eventData.delta.x : 0;
        delta = Mathf.Clamp(delta, -20, 20);

        var localDir = GetLocalDir();

        var d = dirVec * delta * dragSpeed * Time.deltaTime;
        var dir = d.normalized; // 아래에서 너무 작아지기 전에 저장(-0.05f 부분)
        if (delta > 0)
        {
            if (Physics.Raycast(rayPoint_max.transform.position, localDir, out RaycastHit hit, 4f, RayLayer))
            {
                var amount = Vector3.Dot(d, dirVec);
                var amount_min = Mathf.Min(amount, hit.distance - 0.1f);
                d = dirVec * amount_min;
                if (hit.distance < 0.1f)
                {
                    var other = hit.collider.gameObject.GetComponent<Block>();
                    if (other)
                        other.ShakeBlock(dir, delta);
                    d = Vector3.zero;
                }
            }
        }
        else if (delta < 0)
        {
            if (Physics.Raycast(rayPoint_min.transform.position, -localDir, out RaycastHit hit, 4f, RayLayer))
            {
                var amount = Vector3.Dot(d, dirVec);
                var amount_min = Mathf.Min(Mathf.Abs(amount), hit.distance - 0.1f);
                d = dirVec * -amount_min;
                if (hit.distance < 0.1f)
                {
                    var other = hit.collider.gameObject.GetComponent<Block>();
                    if (other)
                        other.ShakeBlock(dir, delta);
                    d = Vector3.zero;
                }
            }
        }
        else
            d = Vector3.zero;

        var newPos = transform.localPosition + d;
        transform.localPosition = newPos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        GameManager.I.isBlockDragging = false;
    }

}
