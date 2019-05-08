using System.ComponentModel;
using UnityEngine;

/// <summary>
/// [Category("")]参数说明：
/// 0：启用
/// 1：第一顺序位加载
/// 2：第二顺序位加载
/// </summary>
public partial class TableManager : MonoBehaviour
{
    public void Awake()
    {
        for (int i = 1; i <= 2; i++)
        {
            Init(i);
            StartCoroutine(Load(false));
        }
    }

    #region Table

    //[Category("0")]
    //public MovieTable MovieTable; //{ get; private set; }
    public AudioTable AudioTable; //{ get; private set; }
    //public LanguageTable LanguageTable; //{ get; private set; }
    //public LocalLanguageTable LocalLanguageTable; //{ get; private set; }
    //public OpenFunctionTable OpenFunctionTable; //{ get; private set; }
    //[Category("0")]
    //public GuideTable GuideTable; //{ get; private set; }
    //public FxTable FxTable; //{ get; private set; }

    //public BasePropertyTable BasePropertyTable; //{ get; private set; }   
    //public HeroTable HeroTable; //{ get; private set; }
    //public EquipTable EquipTable; //{ get; private set; }
    //public ItemTable ItemTable; //{ get; private set; }
    //public SkillTable SkillTable; //{ get; private set; }

    //public TaskTable TaskTable; //{ get; private set; }
    //public GamePropertyTable GamePropertyTable; //{ get; private set; }
    //public DToDTable DToDTable; //{ get; private set; }
    //public DamageTable DamageTable; //{ get; private set; }
    //public BattleLevelTable BattleLevelTable; //{ get; private set; }
    //public BattleActionTable BattleActionTable; //{ get; private set; }
    //public AiTable AiTable; //{ get; private set; }
    //public TaskActionTable TaskActionTable; //{ get; private set; }
    //public DialogueTable DialogueTable; //{ get; private set; }
    //public ShopTable ShopTable; //{ get; private set; }

    //[Category("1")]
    //public MiscTable MiscTable; //{ get; private set; }
    //[Category("1")]
    //public HostTable HostTable; //{ get; private set; }

    #endregion
}