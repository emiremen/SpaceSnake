using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 1f;

    [Range(0, -10)][SerializeField] float spawnDistanceFromHead = -2f;
    [SerializeField] float bodySpeed = 4;
    [SerializeField] int playerLevel = 1;

    [SerializeField] GameObject bodyPartPrefab;
    private List<GameObject> bodyParts = new List<GameObject>();
    List<string> tags = new List<string>();

    bool isMovingRight, isMovingLeft;

    [SerializeField] ParticleSystem eatingParticle;

    private void OnEnable()
    {
        EventManager.SetPlayerLevel += SetPlayerLevel;
        EventManager.GetPlayerLevel += GetPlayerLevel;
        EventManager.isMovingLeftWithButton += isMovingLeftWithButton;
        EventManager.isMovingRihtWithButton += isMovingRihtWithButton;
    }

    private void OnDisable()
    {
        EventManager.SetPlayerLevel -= SetPlayerLevel;
        EventManager.GetPlayerLevel -= GetPlayerLevel;
        EventManager.isMovingLeftWithButton -= isMovingLeftWithButton;
        EventManager.isMovingRihtWithButton -= isMovingRihtWithButton;
    }

    void Start()
    {
        bodySpeed = moveSpeed / 2;
        SnakeGrow();
        for (int i = 1; i <= 5; i++)
        {
            tags.Add("Level " + i + " Props");
        }
    }

    void Update()
    {
        if (EventManager.GetGameState?.Invoke() == GameState.Playing)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

            Movement();
            MoveRightWithButton(isMovingRight);
            MoveLeftWithButton(isMovingLeft);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SnakeGrow();
        }
    }

    private void isMovingRihtWithButton(bool value)
    {
        isMovingRight = value;
    }

    private void isMovingLeftWithButton(bool value)
    {
        isMovingLeft = value;
    }

    int GetPlayerLevel()
    {
        return playerLevel;
    }

    void SetPlayerLevel(int value)
    {
        playerLevel = value;
    }

    private void Movement()
    {

        float rotationDirection = Input.GetAxisRaw("Horizontal");
        transform.Rotate(Vector3.up * rotationDirection * rotationSpeed * Time.deltaTime);

        foreach (var part in bodyParts)
        {
            int index = bodyParts.IndexOf(part);
            Vector3 target = (index == 0) ? transform.position : bodyParts[index - 1].transform.position;

            Vector3 moveDirection = target - part.transform.position;
            part.transform.LookAt(target);
            part.transform.position = Vector3.Lerp(part.transform.position, part.transform.position + moveDirection, bodySpeed * Time.deltaTime);
        }
    }

    private void MoveLeftWithButton(bool isMovingLeft)
    {
        if (!isMovingLeft) return;
        transform.Rotate(Vector3.up * -1 * rotationSpeed * Time.deltaTime);

        foreach (var part in bodyParts)
        {
            int index = bodyParts.IndexOf(part);
            Vector3 target = (index == 0) ? transform.position : bodyParts[index - 1].transform.position;

            Vector3 moveDirection = target - part.transform.position;
            part.transform.LookAt(target);
            part.transform.position = Vector3.Lerp(part.transform.position, part.transform.position + moveDirection, bodySpeed * Time.deltaTime);
        }
    }

    private void MoveRightWithButton(bool isMovingRight)
    {
        if (!isMovingRight) return;
        transform.Rotate(Vector3.up * 1 * rotationSpeed * Time.deltaTime);

        foreach (var part in bodyParts)
        {
            int index = bodyParts.IndexOf(part);
            Vector3 target = (index == 0) ? transform.position : bodyParts[index - 1].transform.position;

            Vector3 moveDirection = target - part.transform.position;
            part.transform.LookAt(target);
            part.transform.position = Vector3.Lerp(part.transform.position, part.transform.position + moveDirection, bodySpeed * Time.deltaTime);
        }
    }

    private void SnakeGrow()
    {
        GameObject spawnedBodyPart = Instantiate(bodyPartPrefab, (bodyParts.Count > 0) ? bodyParts[bodyParts.Count - 1].transform.position + (bodyParts[bodyParts.Count - 1].transform.forward * spawnDistanceFromHead) : transform.position + (transform.forward * spawnDistanceFromHead), Quaternion.identity);
        transform.DOScale(transform.localScale.x + .01f, .4f).From(transform.localScale.x).WaitForCompletion();
        var sequence = DOTween.Sequence();
        foreach (var part in bodyParts)
        {
            //part.transform.DOScale(part.transform.localScale.x + .01f, .4f).From(part.transform.localScale.x);
            sequence.Append(part.transform.DOScale(part.transform.localScale.x + .01f, .4f));
        }
        bodyParts.Add(spawnedBodyPart);
        spawnedBodyPart.transform.DOScale(bodyParts[0].transform.localScale.x, .4f).From(0).WaitForCompletion();
        if (bodyParts.Count > 1)
        {
            Camera.main.transform.DOPunchRotation(new Vector3(.4f,.4f,.4f),.3f);
            SoundManager.instance.PlayAudio("Eat");
            eatingParticle.Play();
            EventManager.GainScore?.Invoke();
        }
    }

    private void SnakeShrink()
    {
        EventManager.ShowGameOverPanel?.Invoke();
        EventManager.SetGameState?.Invoke(GameState.GameOver);
        SoundManager.instance.PlayAudio("GameOver");
        Camera.main.transform.DOPunchPosition(new Vector3(1f, 1f, 1f), .5f);
        StartCoroutine(ShrinkPart());
    }

    IEnumerator ShrinkPart()
    {
        for (int i = bodyParts.Count - 1; i >= 1; i--)
        {
            bodyParts[i].transform.DOScale(0, .5f).From(1).OnComplete(() => Destroy(bodyParts[i])).WaitForCompletion();
            bodyParts.Remove(bodyParts[i]);
            yield return new WaitForSeconds(.08f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        IEnemy enemy = other.GetComponentInChildren<IEnemy>();
        if (enemy != null)
        {
            if (other.CompareTag("Mountains"))
            {
                SnakeShrink();
            }

            string enemyTag = tags.FirstOrDefault(x => x == other.tag);
            if (!string.IsNullOrEmpty(enemyTag))
            {
                if (playerLevel >= int.Parse(other.tag[6].ToString()))
                {
                    other.GetComponent<Collider>().enabled = false;
                    SnakeGrow();

                    other.transform.DOPunchScale(Vector3.one * .2f, .3f).OnComplete(() => other.transform.DOScale(0, .3f).From(1).OnUpdate(() => other.transform.position = Vector3.Lerp(other.transform.position, transform.position, 7f * Time.deltaTime)).OnComplete(() => Destroy(other)));
                }
                else
                {
                    SnakeShrink();
                }
            }


        }
    }

}
