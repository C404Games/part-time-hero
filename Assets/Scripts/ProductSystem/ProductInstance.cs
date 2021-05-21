using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductInstance : MonoBehaviourPun
{
    public int id;

    public Transform holder;

    private Product blueprint;

    public GameObject appearence;

    private float clampSpeed = 10;

    public Rigidbody rigidbody;

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

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
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
    /*
    public void setHolder(Transform holder)
    {
        currentClampTime = 0;
        this.holder = holder;
        if (rigidbody != null && !rigidbody.isKinematic)
            photonView.RPC("disableGravity", RpcTarget.All, photonView.ViewID);
    }
    */

    public void setHolder(string name, bool isTable)
    {
        /*
        currentClampTime = 0;
        this.holder = holder;
        if (rigidbody != null && !rigidbody.isKinematic)
            photonView.RPC("disableGravity", RpcTarget.All, photonView.ViewID);
            */
        //if (id != photonView.ViewID)
        //    return;
        photonView.RPC("setHolderRPC", RpcTarget.All, name, isTable);
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
    /*
    [PunRPC]
    void disableGravity(int id)
    {
        if (id != photonView.ViewID)
            return;
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }
    */
    [PunRPC]
    void setHolderRPC(string name, bool isTable)
    {
        currentClampTime = 0;
        Transform t = null;
        GameObject go = GameObject.Find(name);

        if (isTable)
            t = go == null ? null : go.transform;
        else
            t = go == null ? null : go.transform.GetComponentInChildren<CatcherScript>().transform;
        
        holder = t;

        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    [PunRPC]
    void setAppearence(string appearenceName)
    {
        GameObject product = Resources.Load(appearenceName, typeof(GameObject)) as GameObject;
        PhotonView photonView = product.GetComponent<PhotonView>();
        appearence = Instantiate(
                product,
                Vector3.zero,
                Quaternion.Euler(Vector3.zero)
                );
        appearence.transform.parent = transform;
        appearence.transform.localPosition = new Vector3(0, 0, 0);
        Debug.Log(appearence.ToString());
    }

    [PunRPC]
    void selfDestoyRPC() {
        Destroy(this.gameObject);
    }

    public void selfDestroy()
    {
        photonView.RPC("selfDestroyRPC", RpcTarget.All);
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
