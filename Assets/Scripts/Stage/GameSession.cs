
public static class GameSession
{
    // รูปแบบการเล่น
    public static bool isDuoPlay = false;

    // ตัวละครที่ผู้เล่นเลือก
    public static CharacterData player1Character;
    public static CharacterData player2Character;

    // เริ่มต้นข้อมูลใหม่เมื่อเริ่มรอบใหม่
    public static void ResetSession()
    {
        isDuoPlay = false;

        player1Character = null;
        player2Character = null;
    }
}
