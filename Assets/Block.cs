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


    private void Start()
    {
        dirVec = 
            dir == Direction.x ? Vector3.right :
            dir == Direction.y ? Vector3.up :
            dir == Direction.z ? Vector3.forward : Vector3.zero;

        var dd = Vector3.Dot(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f));
        Debug.Log("dd = " + dd);
        blockSize = Mathf.RoundToInt(Vector3.Dot(dirVec, transform.localScale));

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
        //if (isShaking)
        //    return;
        //Debug.Log("hitPoint = " + hitPoint);

        //isShaking = true;

        //if (Vector3.Dot( transform.position, dirVec) < Vector3.Dot( hitPoint, dirVec)) // �� ������ ���и� ��������
        //{
        //    Debug.Log("���� true");
        //    transform.DORotate(dirVec * 20, 1f);
        //}
        //else
        //{
        //    Debug.Log("���� false");
        //    transform.DORotate(dirVec * -20, 1f);

        //}
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
        delta = Mathf.Clamp(delta, -20, 20);

        var d = dirVec * delta * dragSpeed * Time.deltaTime;
        if (delta > 0)
        {
            if (Physics.Raycast(rayPoint_max.transform.position, dirVec, out RaycastHit hit, 4f))
            {
                var amount = Vector3.Dot(d, dirVec);
                var amount_min = Mathf.Min(amount, hit.distance - 0.1f);
                d = dirVec * amount_min;
                if (hit.distance < 0.1f)
                    d = Vector3.zero;
            }
        }
        else if (delta < 0)
        {
            if (Physics.Raycast(rayPoint_max.transform.position, -dirVec, out RaycastHit hit, 4f))
            {
                var amount = Vector3.Dot(d, dirVec);
                var amount_min = Mathf.Min(Mathf.Abs(amount), hit.distance - 0.1f);
                d = dirVec * -amount_min;
                if (hit.distance < 0.1f)
                    d = Vector3.zero;

            }
        }
        else
            d = Vector3.zero;

        var newPos = transform.position + d;
        transform.position = newPos;
    }




    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
    }

}
