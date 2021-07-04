using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MessagesManager : MonoBehaviour
{
    public static UnityAction<int, bool> UpdateScore;
    [SerializeField] string[] goodMessages;
    [SerializeField] string[] badMessages;
    [SerializeField] string[] funnyMessages;
    [SerializeField] float infoTextDuration;
    ObjectPools objectPools;
    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        objectPools = FindObjectOfType<ObjectPools>();
        if (player == null)
            Debug.LogError("Message Manager could not find an object with 'Player' tag");
        if (objectPools == null)
            Debug.LogError("Message Manager could not find an object with the ObjectPools component on it");
    }

    void OnEnable()
    {
        Meteor.TextInfo += DisplayWorldInfo;
        SimpleDrone.TextInfo += DisplayWorldInfo;
        Building.TextInfo += DisplayWorldInfo;
    }

    void OnDisable()
    {
        Meteor.TextInfo -= DisplayWorldInfo;
        SimpleDrone.TextInfo -= DisplayWorldInfo;
        Building.TextInfo -= DisplayWorldInfo;
    }

    string GetGoodMessage()
    {
        int length = goodMessages.Length;
        int ranNum = Random.Range(0, length);
        for (int i = 0; i < length; i++)
        {
            if (goodMessages[ranNum] != null)
                return goodMessages[ranNum];
        }
        return "Invalid";
    }

    string GetBadMessage()
    {
        int length = badMessages.Length;
        int ranNum = Random.Range(0, length);
        for (int i = 0; i < length; i++)
        {
            if (badMessages[ranNum] != null)
                return badMessages[ranNum];
        }
        return "Invalid";
    }

    string GetFunnyMessage()
    {
        int length = funnyMessages.Length;
        int ranNum = Random.Range(0, length);
        for (int i = 0; i < length; i++)
        {
            if (funnyMessages[ranNum] != null)
                return funnyMessages[ranNum];
        }
        return "Invalid";
    }

    int GetPoints(Vector3 pos)
    {
        if (player != null)
            return Mathf.RoundToInt(Vector3.Distance(pos, player.transform.position));
        return 0;
    }

    /// <param name="messageType"> (0 = Good Message) : (1 = Bad Message) : (2 = Scored Points) : (3 = Lost Points) : (4 = Funny Message) </param>
    void DisplayWorldInfo(GameObject obj, byte messageType)
    {
        if (objectPools != null)
        {
            int points = GetPoints(obj.transform.position);
            GameObject newText = objectPools.GetAvailableInfoLetters();
            newText.SetActive(true);
            newText.transform.position = obj.transform.position + Vector3.up * 20;
            TMP_Text text = newText.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                switch (messageType)
                {
                    case 0:
                        text.color = Color.green;
                        text.text = GetGoodMessage();
                        break;

                    case 1:
                        text.color = new Color(.6f, .4f, .2f);
                        text.text = GetBadMessage();
                        break;

                    case 2:

                        text.color = Color.yellow;
                        text.text = points.ToString();
                        UpdateScore?.Invoke(points, true);
                        break;

                    case 3:

                        text.color = Color.red;
                        text.text = points.ToString();
                        UpdateScore?.Invoke(points, false);
                        break;

                    case 4:

                        text.color = Color.white;
                        text.text = GetFunnyMessage();
                        break;

                    default:
                        text.text = "Craig Hussey Was Here";
                        text.color = Color.Lerp(Color.red, Color.black, text.textInfo.characterCount);
                        break;
                }
            }
            if (player != null)
                newText.transform.LookAt(player.transform.localPosition);
            StartCoroutine(TurnOffGameObject(newText));
        }
    }

    IEnumerator TurnOffGameObject(GameObject target)
    {
        yield return new WaitForSeconds(infoTextDuration);
        if (target.activeInHierarchy)
            target.SetActive(false);
        StopCoroutine(TurnOffGameObject(target));
    }
}