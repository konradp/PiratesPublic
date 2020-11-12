using UnityEngine;
public enum PirateStatus { idle, macarening, loadingGun, running, onLookout}
public class PirateAnimationControl : MonoBehaviour
{
    public PirateStatus status;
    [SerializeField] Animator animCont ;
    [SerializeField] Transform[] runningCheckPoints;
    int curNode;
    float moveSpeed = 1f;
    private void Start() 
    {
        switch (status)
        {
            case(PirateStatus.loadingGun):
                animCont.SetTrigger("isLoadingGun");
                break;
            case(PirateStatus.onLookout):
                animCont.SetTrigger("isLooking");
                break;
            case(PirateStatus.running):
                animCont.SetTrigger("isRunning");
                break;
            default:
                break;
        }    
        curNode=0;
    }
    private void Update()
    {
        if(status==PirateStatus.running)
        {
            transform.LookAt(runningCheckPoints[curNode].position);
            transform.Translate(Vector3.forward * moveSpeed * TimeControl.deltaTime);
            NodeChecker();
        }
    }
    void NodeChecker()
    {
        if ( isOnspot(transform.position,runningCheckPoints[curNode].position))
        {
            curNode++;
        }
        if(curNode>=runningCheckPoints.Length)
            curNode=0;
    }
    bool isOnspot(Vector3 pos, Vector3 target)
    {
        float posx = pos.x;
        float posz = pos.z;
        float tarx = target.x;
        float tarz = target.z;
        if (FastApprox(posx,tarx,0.1f) && FastApprox(posz,tarz,0.1f))
        {
            return true;
        }else
        {
            return false;
        }
    }
    bool FastApprox(float a, float b, float threshold){
        if(threshold>0f)
        {
            return Mathf.Abs(a-b)<= threshold;
        }
        else
        {
            return Mathf.Approximately(a,b);
        }
    }
}
