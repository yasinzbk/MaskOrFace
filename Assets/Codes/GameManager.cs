using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        public Sprite bodySprite;
        public bool hasMask;              // Gercek durumu
        [TextArea] public string dialogue;
        [TextArea] public string trueIntentText;
    }

    [Header("Characters")]
    public List<Character> characters;
    private int currentIndex = 0;

    [Header("UI References")]
    public Image bodyImage;
    public Image maskImage;
    public Image handprintImage;
    public Image hintImage;

    public TMP_Text dialogueText;

    [Header("Counts")]
    private int correctCount = 0;
    private int wrongCount = 0;

    private bool inputLocked = false;


    [Header("End Level UI")]
    public GameObject angelDevilPanel;

    public RectTransform scoreBarRoot;
    public RectTransform indicator;
    public RectTransform greenFill;
    public RectTransform redFill;

    public TMP_Text leftScoreText;
    public TMP_Text rightScoreText;

    public Image resultImage;
    public Sprite angelSprite;
    public Sprite devilSprite;


    void Start()
    {
        ShowCharacter();
    }

    void ShowCharacter()
    {
        inputLocked = false;

        var c = characters[currentIndex];

        bodyImage.sprite = c.bodySprite;
        dialogueText.text = c.dialogue;

        maskImage.gameObject.SetActive(c.hasMask);
        maskImage.transform.localScale = Vector3.one;

        handprintImage.gameObject.SetActive(false);
        hintImage.gameObject.SetActive(false);

        bodyImage.transform.localPosition = new Vector3(800, 0, 0);
        StartCoroutine(SlideIn());
    }

    IEnumerator SlideIn()
    {
        Vector3 start = bodyImage.transform.localPosition;
        Vector3 target = Vector3.zero;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            bodyImage.transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }

    public void OnMaskSelected()
    {
        Debug.Log("Mask Selected"); 

        if (inputLocked) return;
        inputLocked = true;

        var c = characters[currentIndex];

        if (c.hasMask)
        {
            correctCount++;
            StartCoroutine(ShowFeedback_MaskCorrect(c));
        }
        else
        {
            wrongCount++;
            StartCoroutine(ShowFeedback_MaskWrong());
        }

    }

    public void OnFaceSelected()
    {

        Debug.Log("Face Selected");

        if (inputLocked) return;
        inputLocked = true;

        var c = characters[currentIndex];

        if (!c.hasMask)
        {
            correctCount++;
            StartCoroutine(ShowFeedback_FaceCorrect(c));
        }
        else
        {
            wrongCount++;
            StartCoroutine(ShowFeedback_FaceWrong());
        }

    }

    IEnumerator ShowFeedback_MaskCorrect(Character c)
    {
        float t = 0;
        Vector3 startScale = maskImage.transform.localScale;

        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            maskImage.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        maskImage.gameObject.SetActive(false);
        dialogueText.text = c.trueIntentText;

        yield return new WaitForSeconds(1.2f);
        NextCharacter();
    }

    IEnumerator ShowFeedback_MaskWrong()
    {
        handprintImage.gameObject.SetActive(true);
        dialogueText.text = "Yanlis karar verdin.";

        yield return new WaitForSeconds(0.8f);
        NextCharacter();
    }


    IEnumerator ShowFeedback_FaceCorrect(Character c)
    {
        dialogueText.text = c.trueIntentText;

        // Hafif olumlu feedback
        bodyImage.transform.localScale = Vector3.one * 1.05f;
        yield return new WaitForSeconds(0.3f);
        bodyImage.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(0.6f);
        NextCharacter();
    }

    IEnumerator ShowFeedback_FaceWrong()
    {
        hintImage.gameObject.SetActive(true);
        dialogueText.text = "Gozunden kacirilan bir sey vardi...";

        bodyImage.transform.localScale = Vector3.one * 1.1f;
        yield return new WaitForSeconds(1f);
        bodyImage.transform.localScale = Vector3.one;

        NextCharacter();
    }

    void NextCharacter()
    {
        currentIndex++;

        if (currentIndex >= characters.Count)
        {
            EndLevel();
        }
        else
        {
            ShowCharacter();
        }
    }

    void EndLevel()
    {
        Debug.Log("DOGRU: " + correctCount + " | YANLIS: " + wrongCount);

        if (correctCount > wrongCount)
            Debug.Log("MELEK ");
        else
            Debug.Log("SEYTAN ");


        inputLocked = true;

        angelDevilPanel.SetActive(true);

        leftScoreText.text = correctCount.ToString();
        rightScoreText.text = wrongCount.ToString();

        StartCoroutine(AnimateScoreBar());
        // Burada bolum sonu UI'ye gecersin
    }

    IEnumerator AnimateScoreBar()
    {
        int correct = correctCount;
        int wrong = wrongCount;
        int total = Mathf.Max(1, correct + wrong);

        float balance = (float)(correct - wrong) / total; // -1 .. +1

        float barWidth = scoreBarRoot.rect.width;
        float targetX = balance * (barWidth / 2f);

        float duration = 0.7f;
        float t = 0f;

        Vector2 startPos = indicator.anchoredPosition;
        Vector2 targetPos = new Vector2(targetX, startPos.y);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            float x = Mathf.Lerp(startPos.x, targetPos.x, t);
            indicator.anchoredPosition = new Vector2(x, startPos.y);

            float halfWidth = barWidth / 2f;

            float greenWidth = Mathf.Clamp(halfWidth + x, 0, barWidth);
            float redWidth = Mathf.Clamp(halfWidth - x, 0, barWidth);

            greenFill.sizeDelta = new Vector2(greenWidth, greenFill.sizeDelta.y);
            redFill.sizeDelta = new Vector2(redWidth, redFill.sizeDelta.y);

            yield return null;
        }

        // RESULT
        if (correct > wrong)
            resultImage.sprite = angelSprite;
        else
            resultImage.sprite = devilSprite;

        resultImage.gameObject.SetActive(true);
    }

}
