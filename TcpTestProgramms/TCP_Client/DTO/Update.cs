using Shared.Contracts;

namespace TCP_Client.DTO
{
    public class UpdateDTO: IProtocol
    {
        public string _Updated_board;
        public string _Updated_dice_information;
        public string _Updated_turn_information;
    }
}
