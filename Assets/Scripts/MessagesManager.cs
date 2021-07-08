using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MessagesManager : MonoBehaviour
{
    public static UnityAction<int, bool> UpdateScore;
    [SerializeField] string[] goodMessages;
    [SerializeField] string[] badMessages;
    [SerializeField] string[] neutralMessages;
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
        Rocket.TextInfo += DisplayWorldInfo;
        Projectile.TextInfo += DisplayWorldInfo;
    }

    void OnDisable()
    {
        Meteor.TextInfo -= DisplayWorldInfo;
        SimpleDrone.TextInfo -= DisplayWorldInfo;
        Building.TextInfo -= DisplayWorldInfo;
        Rocket.TextInfo -= DisplayWorldInfo;
        Projectile.TextInfo -= DisplayWorldInfo;
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

    string GetNeutralMessage()
    {
        int length = neutralMessages.Length;
        int ranNum = Random.Range(0, length);
        for (int i = 0; i < length; i++)
        {
            if (neutralMessages[ranNum] != null)
                return neutralMessages[ranNum];
        }
        return "Invalid";
    }

    int GetPointsByDistance(Vector3 pos)
    {
        if (player != null)
            return Mathf.RoundToInt(Vector3.Distance(pos, player.transform.position));
        return 0;
    }

    int GetPointsByDamage(float damage)
    {
        if (player != null)
            return Mathf.RoundToInt(damage);
        return 0;
    }

    /// <param name="messageType"> (0 = Good Message) : (1 = Bad Message) : (2 = Scored Points) : (3 = Lost Points) : (4 = Funny Message) </param>
    /// <param name="pointType"> (0 = Points by distance) : (1 = Points by damage) </param>
    void DisplayWorldInfo(GameObject obj, byte messageType, byte pointType = 0, float damage = 0)
    {
        int points = 0;
        if (objectPools != null)
        {
            if (pointType.Equals(0))
                points = GetPointsByDistance(obj.transform.position);
            if (pointType.Equals(1))
                points = GetPointsByDamage(damage);
            GameObject newText = objectPools.GetAvailableInfoLetters();
            newText.SetActive(true);
            newText.transform.position = obj.transform.position + Vector3.up * 20;
            TMP_Text text = newText.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                switch (messageType)
                {
                    case 0:
                        text.color = Color.yellow;
                        text.text = GetGoodMessage();
                        break;

                    case 1:
                        text.color = Color.Lerp(Color.red, Color.black, text.textInfo.characterCount);
                        text.text = GetBadMessage();
                        break;

                    case 2:
                        text.color = Color.green;
                        text.text = points.ToString();
                        UpdateScore?.Invoke(points, true);
                        break;

                    case 3:

                        text.color = Color.red;
                        text.text = points.ToString();
                        UpdateScore?.Invoke(points, false);
                        break;

                    case 4:

                        text.color = new Color(.6f, .4f, .2f);
                        text.text = GetNeutralMessage();
                        break;

                    default:
                        text.text = "Craig Hussey Was Here";
                        text.color = new Color(.8f, .7f, .4f);
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