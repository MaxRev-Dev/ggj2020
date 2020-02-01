using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

public class BuildingBlock : MonoBehaviour
{
    private BlockTransform target;

    public void Update()
    {
        SetTransformInternal(target);
    }
    public string GetId()
    {
        return this.tag + '.' + this.name;
    }

    public IEnumerator FromMovement(HistoryContainer container, float percentage)
    {
        yield return StartCoroutine(StartMovement(container, percentage));
    }

    private IEnumerator StartMovement(HistoryContainer container, float percentage)
    {
        foreach (var blockTransform in container.GetForPercentage(GetId(), percentage))
        {
            SetTransformInternal(blockTransform);
            yield return 0;
        }
        SetZeroVelocity(this.gameObject);
    }


    private void SetZeroVelocity(GameObject block)
    {
        block.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }

    //public void SetTransform(GameObject item, BlockTransform itemTransform)
    //{
    //    item.transform.position = Vector3.MoveTowards(item.transform.position, itemTransform.Position, 1);
    //    item.transform.rotation = Quaternion.RotateTowards(item.transform.rotation, itemTransform.Rotation, 1);
    //}

    public IEnumerator FromInstantMovement(HistoryContainer movements, float percentage)
    {
        yield return StartCoroutine(MoveInstant(movements, percentage));
    }

    private IEnumerator MoveInstant(HistoryContainer movements, float percentage)
    {
        BlockTransform current = null;
        foreach (var blockTransform in movements.GetForPercentage(GetId(), percentage))
        {
            current = blockTransform;
            yield return 0;
        }

        if (current != null) SetTransformInternal(current);

        SetZeroVelocity(this.gameObject);
    }

    internal void SetTransformInternal(BlockTransform itemTransform)
    {
        if (itemTransform == default) return;
        var item = this;

        item.transform.position = itemTransform.Position;
        item.transform.rotation = itemTransform.Rotation;
    }
    internal void SetTransform(BlockTransform itemsTransform)
    {
        target = itemsTransform;
    }
}