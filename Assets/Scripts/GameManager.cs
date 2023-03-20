using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Beyblade player;
    [SerializeField] private Beyblade enemy;

    [SerializeField] private List<Transform> spawnPoints;

    [SerializeField] private float launchingForce;

    [SerializeField] private LineDrawer lineDrawer;

    [SerializeField] private GameObject gameResultObject;
    [SerializeField] private TextMeshProUGUI gameResultText;

    [SerializeField] private GameObject bendDirectionObject;
    [SerializeField] private Image bendDirectionIcon;
    [SerializeField] private Sprite[] bendDirecionSprites;

    [SerializeField] private GameObject instructionObject;

    [SerializeField] private GameObject creditPage;

    [SerializeField] private Animator transition;

    public static bool GameStarted { get; private set; }
    public static Action OnGameStarted;

    private Coroutine gameResultCoroutine;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        transition.gameObject.SetActive(true);
        Invoke(nameof(HideTransition), 0.5f);
        instructionObject.SetActive(false);
    }
    private void HideTransition()
    {
        transition.gameObject.SetActive(false);
    }

    private void Start()
    {
        foreach (var b in Beyblade.beyblades)
        {
            b.OnSpinningStopped += BeybladeStoppedMoving;
            PickSpawnPoint(b);
        }
    }
    private void PickSpawnPoint(Beyblade beyblade)
    {
        int index = UnityEngine.Random.Range(0, spawnPoints.Count);
        beyblade.transform.position = spawnPoints[index].position;
        spawnPoints.RemoveAt(index);
    }

    private void OnDestroy()
    {
        GameStarted = false;
    }

    public void SetCreditPage(bool active)
    {
        creditPage.SetActive(active);
    }

    public void OutOfBound(Beyblade beyblade)
    {
        if (gameResultCoroutine != null) return;

        gameResultCoroutine = StartCoroutine(GameResult(beyblade, true));
    }
    private void BeybladeStoppedMoving(Beyblade beyblade)
    {
        if (gameResultCoroutine != null) return;

        gameResultCoroutine = StartCoroutine(GameResult(beyblade));
    }
    private IEnumerator GameResult(Beyblade beyblade, bool outOfBounds = false)
    {
        yield return new WaitForSeconds(2f);

        instructionObject.SetActive(false);
        gameResultObject.SetActive(true);
        gameResultText.SetText(outOfBounds ? "Out of bounds" : beyblade == enemy ? "You win" : "You lose");

        yield return new WaitForSeconds(3f);

        transition.gameObject.SetActive(true);
        transition.Play("Transition_Out", 0, 0);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (GameStarted) return;

        UpdateBendDirectionIcon(Input.GetAxisRaw("Horizontal"));

        if (Input.GetMouseButtonDown(0))
        {
            GameStarted = true;
            bendDirectionObject.SetActive(false);
            instructionObject.SetActive(true);

            player.ReleaseBeyblade(lineDrawer.AimingPosition * launchingForce, 500f);
            enemy.ReleaseBeyblade((player.transform.position - enemy.transform.position).normalized * launchingForce, 500f);
            OnGameStarted?.Invoke();
        }
    }
    private void UpdateBendDirectionIcon(float input)
    {
        if (input < 0f)
        {
            bendDirectionIcon.sprite = bendDirecionSprites[2];
        }
        else if (input > 0f)
        {
            bendDirectionIcon.sprite = bendDirecionSprites[1];
        }
        else
        {
            bendDirectionIcon.sprite = bendDirecionSprites[0];
        }
    }
}