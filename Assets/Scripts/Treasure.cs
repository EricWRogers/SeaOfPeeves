using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Treasure : MonoBehaviour
{
    public string collectSoundTag = "collect";
    public GameObject seagulPrefab;
    public float seagulHeight = 50f;
    public GameObject interactPrompt;
    public float interactDistance = 5f;
    private bool interactable = false;

    private PlayerInputActions playerControls;
    private InputAction interact;

    public void Collect(InputAction.CallbackContext context)
    {
        if (!interactable)
        {
            return;
        }

        interact.performed -= Collect;
        GameManager.Instance.IncrementScore(1);
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySting(collectSoundTag);
        }
        Destroy(this.gameObject);
    }

    public void Awake()
    {

        playerControls = new PlayerInputActions();

        interact = playerControls.Player.Interact;
        interact.Enable();

        interact.performed += Collect;

        GameObject seagul = Instantiate(seagulPrefab);
        seagul.transform.position = new Vector3(this.transform.position.x, 18 + seagulHeight, this.transform.position.z);

    }


    public void Update()
    {
        if(GameManager.Instance != null && GameManager.Instance.player != null)
        {
            if(Vector3.Distance(this.transform.position, GameManager.Instance.player.transform.position) <= interactDistance)
            {
                interactable = true;
            }
            else
            {
                interactable = false;
            }
        }

        interactPrompt.SetActive(interactable);
    }


}
