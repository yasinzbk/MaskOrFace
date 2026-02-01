using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Character
    {
        // --- Kimlik ---
        public string id;

        // --- Görseller ---
        public Sprite maskedBody;      // Mask takili tam vucut
        public Sprite unmaskedBody;    // Maskesiz tam vucut

        // --- Mantik ---
        public bool hasMask;           // Bu karakter maskeli mi?

        // --- Metinler ---
        [TextArea] public string dialogue;
        [TextArea] public string trueIntentText;

        // --- Pozisyon Referanslari (CharacterCard LOCAL SPACE) ---

        // El animasyonunun gidecegi yer
        //public Vector2 headPosition;

        // Face yanlis / Mask dogru disi hint durumu
        //public Vector2 hintPosition;

        public Vector2 headOffset;

        public Vector2 hintOffset;
        public Vector2 hintSize;
    }

    [Header("Characters")]
    public List<Character> characters;
    private int currentIndex = 0;

    [Header("UI References")]
    public Image bodyImage;
    //public Image maskImage;
    public Image handprintImage;
    public Image hintImage;

    public TMP_Text dialogueText;
    public TMP_Text dialogueTextFinal;


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

    [Header("Main Character")]
    public Image resultImage;
    public Image playerBodyImage;

    public Sprite playerAngelSprite;
    public Sprite playerDevilSprite;
    public Sprite playerMaskedSprite;

    public Transform playerPosition;
    public Vector2 headOffsetPlayer;

    [Header("Hand Animation")]
    public RectTransform handImage;
    public RectTransform handTargetPoint;

    float endScaleValue = 2.8f;
    float startScaleValue = 4.5f;

    [Header("End Level Dialogue")]
    [TextArea] public string angelEndDialogue;
    [TextArea] public string devilEndDialogue;


    [Header("Intro")]
    public GameObject introPanel;
    public Image introBodyImage;
    [TextArea] public List<string> introDialogues;

    public Button nextButton;
    public Button skipButton;

    private int introIndex = 0;
    private bool inIntro = true;


    void Start()
    {
        //ShowCharacter();

        StartIntro();
    }

    void StartIntro()
    {
        inIntro = true;
        inputLocked = true;

        introPanel.SetActive(true);

        introBodyImage.sprite = playerMaskedSprite;
        introBodyImage.transform.localPosition = new Vector3(800, 0, 0);

        introIndex = 0;

        StartCoroutine(IntroSlideIn());
    }

    IEnumerator IntroSlideIn()
    {
        Vector3 start = introBodyImage.transform.localPosition;
        Vector3 target = Vector3.zero;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            introBodyImage.transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        ShowIntroDialogue();
    }


    void ShowIntroDialogue()
    {
        dialogueText.text = introDialogues[introIndex];

        // hafif öne gelme / pulse
        introBodyImage.transform.DOKill();
        introBodyImage.transform.localScale = Vector3.one;

        introBodyImage.transform
            .DOScale(1.05f, 0.2f)
            .SetLoops(2, LoopType.Yoyo);
    }

    public void OnIntroNext()
    {
        if (!inIntro) return;

        introIndex++;

        if (introIndex >= introDialogues.Count)
        {
            EndIntro();
        }
        else
        {
            ShowIntroDialogue();
        }
    }

    public void OnIntroSkip()
    {
        if (!inIntro) return;
        EndIntro();
    }

    void EndIntro()
    {
        inIntro = false;

        introPanel.SetActive(false);

        inputLocked = false;

        // ANA OYUN BAÞLASIN
        currentIndex = 0;
        ShowCharacter();
    }


    void ShowCharacter()
    {
        //inputLocked = false;

        var c = characters[currentIndex];


        bodyImage.sprite = c.maskedBody;

  

        dialogueText.text = c.dialogue;

        //maskImage.gameObject.SetActive(c.hasMask);
        //maskImage.transform.localScale = Vector3.one;

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

        inputLocked = false;

    }

    public void OnMaskSelected()
    {
        Debug.Log("Mask Selected");

        //if (inputLocked) return;
        //inputLocked = true;

        //var c = characters[currentIndex];

        //if (c.hasMask)
        //{
        //    correctCount++;
        //    StartCoroutine(ShowFeedback_MaskCorrect(c));
        //}
        //else
        //{
        //    wrongCount++;
        //    StartCoroutine(ShowFeedback_MaskWrong());
        //}

        if (inputLocked) return;
        inputLocked = true;

        var c = characters[currentIndex];

        if (c.hasMask)
        {
            correctCount++;
            StartCoroutine(HandleMaskTrue(c));
        }
        else
        {
            wrongCount++;
            StartCoroutine(HandleMaskWrong_Face(c));
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

    IEnumerator HandleMaskTrue(Character c)
    {
        // 1. El’i ekrana getir
        handImage.gameObject.SetActive(true);

        //Vector2 screenCenter = Vector2.zero;
    //    Vector2 headPos =
    //bodyImage.rectTransform.anchoredPosition + c.headOffset;

        // Karakter local  canvas local
        Vector2 headCanvasPos = handTargetPoint.position;

        //yield return StartCoroutine(MoveHand(screenCenter, headCanvasPos));
        yield return StartCoroutine(HandReachToHead(headCanvasPos));
        yield return new WaitForSeconds(0.7f);

        // 2. Kafada bekle
        //yield return new WaitForSeconds(c.handPauseTime);

        // 3. SPRITE SWAP (KRÝTÝK AN)
        bodyImage.sprite = c.unmaskedBody;

        //// 4. El geri ciksin
        //yield return StartCoroutine(MoveHand(headCanvasPos, screenCenter));
        yield return StartCoroutine(HandPullBack());

        handImage.gameObject.SetActive(false);

        //// 5. Devam
        yield return new WaitForSeconds(1.6f);
        NextCharacter();
    }

    IEnumerator HandReachToHead(Vector2 headCanvasPos)
    {
        handImage.gameObject.SetActive(true);

        //float endScaleValue = 2.8f;
        //float startScaleValue = 4.5f;
        // Baslangic
        handImage.position = new Vector2(300, -300);
        handImage.localScale = Vector3.one * startScaleValue;

        float duration = 1.2f;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // Pozisyon: sag alttan kafaya
            handImage.position =
                Vector2.Lerp(handImage.position, headCanvasPos, t);

            // Scale: buyukten kucuge (perspektif)
            handImage.localScale =
                Vector3.Lerp(Vector3.one * startScaleValue, Vector3.one * endScaleValue, t);

            yield return null;
        }

        // Netle
        handImage.position = headCanvasPos;
        handImage.localScale = Vector3.one * endScaleValue;
    }

    IEnumerator HandleMaskWrong_Face(Character c)
    {
        // El aktif
        handImage.gameObject.SetActive(true);

        Vector2 headCanvasPos = handTargetPoint.position+ (Vector3)c.headOffset;

        // El kafaya gider
        yield return StartCoroutine(HandReachToHead(headCanvasPos));

        // Kýsa bekleme
        yield return new WaitForSeconds(0.3f);

        // SPRITE SWAP YOK 
        // SADECE EL IZI
        handprintImage.gameObject.SetActive(true);

        // El izi kafada olsun
        handprintImage.rectTransform.position =
            bodyImage.rectTransform.position + (Vector3)c.headOffset;

        dialogueText.text = "There was no mask, you fool...";

        // El geri gider
        yield return StartCoroutine(HandPullBack());

        yield return new WaitForSeconds(1.2f);

        handprintImage.gameObject.SetActive(false);

        NextCharacter();
    }


    IEnumerator HandPullBack()
    {
        Vector2 endPos = new Vector2(300, -300);
        Vector3 endScale = Vector3.one * startScaleValue;

        float duration = 0.1f;
        float t = 0f;

        Vector2 startPos = handImage.position;
        Vector3 startScale = handImage.localScale;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            handImage.position =
                Vector2.Lerp(startPos, endPos, t);

            handImage.localScale =
                Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        handImage.gameObject.SetActive(false);
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
        //hintImage.gameObject.SetActive(true);
        //dialogueText.text = "Gozunden kacirilan bir sey vardi...";

        //bodyImage.transform.localScale = Vector3.one * 1.1f;
        //yield return new WaitForSeconds(1f);
        //bodyImage.transform.localScale = Vector3.one;

        //NextCharacter();

        hintImage.transform.DOKill();

        var c = characters[currentIndex];

        dialogueText.text = "You missed something...";

        // Pozisyon ve boyut
        hintImage.rectTransform.position =
            bodyImage.rectTransform.position + (Vector3)c.hintOffset;

        hintImage.rectTransform.sizeDelta = c.hintSize;

        hintImage.gameObject.SetActive(true);
        hintImage.transform.localScale = Vector3.one;

        // DOTWEEN PULSE (tekrar eden)
        Tween pulseTween = hintImage.transform
            .DOScale(1.15f, 0.45f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Daha uzun bekleme
        yield return new WaitForSeconds(2.5f);

        // Temizle
        pulseTween.Kill();
        hintImage.transform.localScale = Vector3.one;
        hintImage.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

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


        leftScoreText.text = correctCount.ToString();
        rightScoreText.text = wrongCount.ToString();

        //angelDevilPanel.SetActive(true);
        //StartCoroutine(AnimateScoreBar());
        //// Burada bolum sonu UI'ye gecersin
        ///
        angelDevilPanel.SetActive(true);

        // Oyuncu BASLANGICTA MASKELI
        playerBodyImage.sprite = playerMaskedSprite;

        StartCoroutine(EndLevelSequence());
    }

    IEnumerator EndLevelSequence()
    {
        // 1. Player saðdan gelsin
        yield return StartCoroutine(SlideInEndPlayer());

        // 2. Skor oynasýn
        yield return StartCoroutine(AnimateScoreBar());

        // Kisa dramatik bekleme
        yield return new WaitForSeconds(0.6f);

        // 4. Maske açýlma (ayný el sistemi)
        yield return StartCoroutine(EndLevelMaskReveal());

        // Burada istersen continue / restart butonu acarsin
    }

    IEnumerator SlideInEndPlayer()
    {
        Vector3 start = playerBodyImage.transform.localPosition;
        Vector3 target = playerBodyImage.transform.localPosition + playerPosition.localPosition;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            playerBodyImage.transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }
    }


    IEnumerator EndLevelMaskReveal()
    {
        Vector2 headPos = handTargetPoint.position + (Vector3)headOffsetPlayer;

        // El gelir
        yield return StartCoroutine(HandReachToHead(headPos));

        yield return new WaitForSeconds(0.5f);

        bool isAngel = correctCount > wrongCount;

        // MASK DUSUYOR
        if (isAngel)
            playerBodyImage.sprite = playerAngelSprite;
        else
            playerBodyImage.sprite = playerDevilSprite;

        // El geri gider
        yield return StartCoroutine(HandPullBack());

        // END LEVEL KONUÞMA
        yield return new WaitForSeconds(0.4f);

        dialogueTextFinal.text = isAngel ? angelEndDialogue : devilEndDialogue;

        dialogueText.alpha = 0;
        dialogueText.DOFade(1f, 0.5f);
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

        //int correct = correctCount;
        //int wrong = wrongCount;
        //int total = Mathf.Max(1, correct + wrong);

        //float balance = (float)(correct - wrong) / total; // -1 .. +1

        //float barWidth = scoreBarRoot.rect.width;
        //float targetX = balance * (barWidth / 2f);

        //float duration = 0.7f;
        //float t = 0f;

        //Vector2 startPos = indicator.anchoredPosition;
        //Vector2 targetPos = new Vector2(targetX, startPos.y);

        //while (t < 1f)
        //{
        //    t += Time.deltaTime / duration;

        //    float x = Mathf.Lerp(startPos.x, targetPos.x, t);
        //    indicator.anchoredPosition = new Vector2(x, startPos.y);

        //    float halfWidth = barWidth / 2f;

        //    float greenWidth = Mathf.Clamp(halfWidth + x, 0, barWidth);
        //    float redWidth = Mathf.Clamp(halfWidth - x, 0, barWidth);

        //    greenFill.sizeDelta = new Vector2(greenWidth, greenFill.sizeDelta.y);
        //    redFill.sizeDelta = new Vector2(redWidth, redFill.sizeDelta.y);

        //    yield return null;
        //}

        //// RESULT
        //if (correct > wrong)
        //    resultImage.sprite = playerAngelSprite;
        //else
        //    resultImage.sprite = playerDevilSprite;

        //resultImage.gameObject.SetActive(true);
    }

}
