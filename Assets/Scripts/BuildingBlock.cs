using Assets.Scripts;
using System.Collections;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{ 

    public void Update()
    { 
    }
    public string GetId()
    {
        return this.tag + '.' + this.name;
    }
     
     
     
    //public void SetTransform(GameObject item, BlockTransform itemTransform)
    //{
    //    item.transform.position = Vector3.MoveTowards(item.transform.position, itemTransform.Position, 1);
    //    item.transform.rotation = Quaternion.RotateTowards(item.transform.rotation, itemTransform.Rotation, 1);
    //}
     
     
}