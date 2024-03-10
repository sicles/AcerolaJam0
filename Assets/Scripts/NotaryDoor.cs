using UnityEngine;

public class NotaryDoor : MonoBehaviour
{
    private static readonly int IsOpened = Animator.StringToHash("IsOpened");
    
    public void OpenDoor()
    {
        transform.parent.GetComponent<Animator>().SetBool(IsOpened, true);
    }
}
