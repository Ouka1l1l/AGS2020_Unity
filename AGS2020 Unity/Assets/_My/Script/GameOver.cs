using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _arrivalFloorText;

    [SerializeField]
    private TextMeshProUGUI _conversionText;

    [SerializeField]
    private TextMeshProUGUI _partsText;

    [SerializeField]
    private TextMeshProUGUI _addPartsText;

    private void OnEnable()
    {
        DungeonManager.instance.PauseStart();

        _arrivalFloorText.text = string.Format("{0:d}階まで到達した", DungeonManager.instance._hierarchy);

        _conversionText.gameObject.SetActive(false);
        _partsText.gameObject.SetActive(false);
        _addPartsText.gameObject.SetActive(false);

        StartCoroutine(GameOverFunc());
    }

    public IEnumerator GameOverFunc()
    {
        yield return null;

        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));

        _conversionText.gameObject.SetActive(true);
        _partsText.gameObject.SetActive(true);

        _partsText.text = string.Format("パーツ {0:d}", SaveData.instance._playerData.parts);

        var itemList = DungeonManager.instance._player._itemList;

        int addParts = 5 * itemList.Count;

        _addPartsText.gameObject.SetActive(true);

        _addPartsText.text = string.Format("+{0:d}", addParts);

        yield return null;

        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));

        _addPartsText.gameObject.SetActive(false);

        SaveData.instance._playerData.parts += addParts;

        _partsText.text = string.Format("パーツ {0:d}", SaveData.instance._playerData.parts);

        yield return null;

        yield return new WaitUntil(() => Input.GetButtonDown("Submit"));

        SceneManager.LoadScene("Hub");
    }

    public void GameQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
    }
}
