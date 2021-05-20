﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductInstance : MonoBehaviourPun
{
    public int id;

    private Transform holder;

    private Product blueprint;

    private GameObject appearence;

    private float clampSpeed = 10;

    private Rigidbody rigidbody;

    private float clampTime = 0.5f;
    private float currentClampTime = 0;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    #region MonoBehavior
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        blueprint = ProductManager.productBlueprints[id];
        updateAppearence();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (holder != null)
        {
            Vector3 newPos = new Vector3();
            if (currentClampTime < clampTime) {
                currentClampTime += Time.deltaTime;
                newPos = Vector3.Lerp(transform.position, holder.transform.position, clampSpeed * Time.deltaTime);
            }
            else
            {
                newPos = holder.transform.position;
            }
            rigidbody.MovePosition(newPos);
        }

    }
    #endregion

    public ProductType getProductType()
    {
        return blueprint.type;
    }

    public void setHolder(Transform holder)
    {
        currentClampTime = 0;
        this.holder = holder;
    }

    public Transform getHolder()
    {
        return holder;
    }

    public bool isHeld()
    {
        return holder != null && holder.GetComponent<StationInstance>() != null;
    }



    // Si hay alguna transición con este producto, se hace
    public bool applyResource(int resourceId)
    {
        foreach (Transition t in blueprint.transitions)
        {
            if (t.src == resourceId)
            {
                if (t.time > 0)
                    StartCoroutine(transformProductDelay(t.dst, t.time));
                else
                    transformProduct(t.dst);
                return true;
            }
        }
        return false;
    }

    public IEnumerator transformProductDelay(int dst, float time)
    {
        yield return new WaitForSeconds(time);
        transformProduct(dst);
    }

    public void transformProduct(int dst)
    {
        id = dst;
        blueprint = ProductManager.productBlueprints[id];
        nextStep();
    }

    [PunRPC]
    void setAppearence(string appearenceName)
    {
        GameObject product = Resources.Load(appearenceName, typeof(GameObject)) as GameObject;
        appearence = Instantiate(
                product,
                Vector3.zero,
                Quaternion.Euler(Vector3.zero)
                );
        appearence.transform.parent = transform;
        appearence.transform.localPosition = new Vector3(0, 0, 0);
        Debug.Log(appearence.ToString());
    }

    //OGM Coger productos
    private void updateAppearence()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (appearence != null)
            PhotonNetwork.Destroy(appearence);
        if (blueprint.appearence != null)
        {
            photonView.RPC("setAppearence", RpcTarget.All, blueprint.appearence);

        }
    }




    private void nextStep()
    {
        photonView.RPC("destoyAppearence", RpcTarget.All);
        photonView.RPC("setAppearence", RpcTarget.All, blueprint.appearence);
    }

    [PunRPC]
    void destoyAppearence()
    {
        if (appearence != null)
            Destroy(appearence);
    }
}
