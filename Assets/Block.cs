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

    Vector3 d; // �̵��� ��Ÿ

    private void Start()
    {
        dirVec = 
            dir == Direction.x ? Vector3.right :
            dir == Direction.y ? Vector3.up :
            dir == Direction.z ? Vector3.forward : Vector3.zero;

        blockSize = Mathf.RoundToInt(Vector3.Dot(dirVec, transform.localScale));

        d = Vector3.zero;

        // �� ���κп� ����ĳ��Ʈ ����Ʈ
        var rayPoint = new GameObject("rayPoint_max");
        rayPoint.transform.SetParent(this.transform);
        rayPoint.transform.localPosition = dirVec /2; // ���� �� ���κ�. (blockSize / 2f) / (float)blockSize �̰� �����ϸ� �׻� 1/2 �̴�
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

        if (Vector3.Dot( transform.position, dirVec) < Vector3.Dot( hitPoint, dirVec)) // �� ������ ���и� ��������
        {
            Debug.Log("���� true");
            transform.DORotate(dirVec * 20, 1f);
        }
        else
        {
            Debug.Log("���� false");
            transform.DORotate(dirVec * -20, 1f);

        }
    }

    // TODO: ���濡�� ���� hitPoint �� ���� ȸ�� �� ���ϱ�
    //Vector3 GetRotationAxisByDir()
    //{
    //    var delta =
    //        dir == Direction.x ? -eventData.delta.x :
    //        dir == Direction.y ? eventData.delta.y :
    //        dir == Direction.z ? Vector3.right : Vector3.right;
    //}

    // TODO: �巡�� �����ϸ� �վ������ delta�� �ʹ� ���̴ϱ� ������ ������ ����
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
