using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Button btn_warning;
    public Button btn_edited;
    public Button btn_completed;

    public Button btn_pause;
    public Button btn_rewind;
    public Button btn_turnStep_left;
    public Button btn_turnStep_right;

    private GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        btn_pause.onClick.AddListener(OnPauseClick);
        //btn_warning.onClick.AddListener(OnWarningClick);
        btn_rewind.onClick.AddListener(OnRewindClick);
       // btn_edited.onClick.AddListener(OnEditedClick);
        //btn_completed.onClick.AddListener(OnCompletedClick);
        btn_turnStep_left.onClick.AddListener(OnTurnLeftClick);
        btn_turnStep_right.onClick.AddListener(OnTurnRightClick);
        manager = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTurnRightClick()
    {
        manager.TurnActiveObjectRight();
    }

    private void OnTurnLeftClick()
    { 
        manager.TurnActiveObjectLeft();
    }

    #region Functional Array
     
    private void OnCompletedClick()
    { 

    }

    private void OnEditedClick()
    { 

    }

    private void OnWarningClick()
    { 

    } 

    #endregion

    private void OnRewindClick()
    {
        manager.RewindOnce();
    }

    private void OnPauseClick()
    {  
        // in menu
    }
}
