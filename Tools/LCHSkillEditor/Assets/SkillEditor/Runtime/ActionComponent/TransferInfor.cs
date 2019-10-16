using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferInfor : MonoBehaviour {

    public Transform target;
    public float height = 5f;
    public float speed = 10f;

    private void Start()
    {
        if (gameObject.layer < 2)
        {

            gameObject.layer = GlobalCamp.globalCampInformations[0].other;
        }

        LCharacterHitDataCmp hdc = GetComponent<LCharacterHitDataCmp>();
        if (null == hdc)
        {
            hdc = gameObject.AddComponent<LCharacterHitDataCmp>();
        }
        hdc.needTouchState = false;
        LCharacterColliderData cdata = hdc.data;

        LCharacterTransferData data = new LCharacterTransferData();
        cdata.type = "tra";
        data.target = target;
        data.height = height;
        data.speed = speed;
        cdata.data = data;

        Collider c = gameObject.GetComponent<Collider>();
        if (null == c)
        {
            BoxCollider b = gameObject.AddComponent<BoxCollider>();
            b.isTrigger = true;
        }
        else
        {
            c.isTrigger = true;
        }


    }

}
