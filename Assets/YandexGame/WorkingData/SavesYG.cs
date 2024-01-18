
namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        // "Технические сохранения" для работы плагина (Не удалять)
        public int idSave;
        public bool isFirstSession = true;
        public string language = "ru";
        public bool promptDone;

        public int score = 1;
        public bool isNeedMusic = true;
        public bool isNeedCellHatch = true;
        public int currentDifficultIndex = 0;
        
        public SavesYG()
        {
            
        }
    }
}
