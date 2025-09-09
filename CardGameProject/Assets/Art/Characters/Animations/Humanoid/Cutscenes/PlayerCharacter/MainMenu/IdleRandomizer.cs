using UnityEngine;

public class IdleRandomizer : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string baseIdleStateName = "MenuIdleBase";
    [SerializeField] private int minLoops = 3;
    [SerializeField] private int maxLoops = 8;

    private Animator anim;
    private int loopsRemaining;

    private readonly string[] variantTriggers =
        { "BasicIdle_Player", "MenuIdleVariantB", "MenuIdleVariantC", "MenuIdleVariantD" };

    private int lastVariantIndex = -1; // -1 means "none played yet"

    void Start()
    {
        anim = GetComponent<Animator>();
        ResetLoopCount();
    }

    void Update()
    {
        AnimatorStateInfo st = anim.GetCurrentAnimatorStateInfo(0);

        if (st.IsName(baseIdleStateName))
        {
            if (st.normalizedTime >= 1f)
            {
                int wholeLoops = Mathf.FloorToInt(st.normalizedTime);
                if (wholeLoops > 0)
                {
                    loopsRemaining -= wholeLoops;

                    if (loopsRemaining <= 0)
                    {
                        TriggerRandomVariant();
                        ResetLoopCount();
                    }
                }
            }
        }
    }

    void ResetLoopCount()
    {
        loopsRemaining = Random.Range(minLoops, maxLoops + 1);
    }

    void TriggerRandomVariant()
    {
        // Clear old triggers
        foreach (string trig in variantTriggers)
            anim.ResetTrigger(trig);

        // Pick random variant not equal to last one
        int idx;
        do
        {
            idx = Random.Range(0, variantTriggers.Length);
        } while (idx == lastVariantIndex && variantTriggers.Length > 1);

        lastVariantIndex = idx;

        anim.SetTrigger(variantTriggers[idx]);
    }

    public void TriggerSwordShatter()
    {
        foreach (string trig in variantTriggers)
            anim.ResetTrigger(trig);

        anim.ResetTrigger("SwordShatter");
        anim.SetTrigger("SwordShatter");
    }
}