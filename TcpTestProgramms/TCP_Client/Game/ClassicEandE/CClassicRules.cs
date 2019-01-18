using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TCP_Client.GameStuff.EandEContracts;
using TCP_Client.GameStuff.XML_Config;

namespace TCP_Client.GameStuff.ClassicEandE
{

	public class ClassicRules : IRules
    {
        private long _idCounter;

        private readonly Game _game;
        private readonly IConfigurationProvider _configurationProvider;

        private Dictionary<EntityType, Func<XElement, IEntity>> _entityFactory = new Dictionary<EntityType, Func<XElement, IEntity>>();

        public int NumberOfPawns { get; } = 2;
        public int DiceSides { get; } = 6;
        public int DiceResult { get; set; }


        public ClassicRules(Game game, IConfigurationProvider configurationProvider)
        {
            _game = game;
            _configurationProvider = configurationProvider;

            _entityFactory = new Dictionary<EntityType, Func<XElement, IEntity>>
            {
                { EntityType.Eel, (config) => CreateEel(config)},
                { EntityType.Escalator, (config) => CreateEscalator(config) },
            };
        }

        public ClassicRules(Game game)
             : this(game, new ConfigurationProvider())
        { }
        

        public void SetupEntitites()
        {
            _game.Board = CreateBoard();

            try
            {
                var configurations = _configurationProvider.GetEntityConfigurations();
                configurations.ForEach(config =>
                {
                    var entityType = (EntityType)config.Get("entitytype", Convert.ToInt32);
                    if (entityType == EntityType.Pawn)
                        _game.Board.Pawns.Add(CreatePawn(config));
                    else
                        _game.Board.Entities.Add(_entityFactory[entityType](config));
                });
            }
            catch 
            {
                throw new Exception();
            }
        }

        public IBoard CreateBoard()
        {
            return new ClassicBoard();
        }

        public IPawn CreatePawn(XElement configuration)
        {
            try
            {

            return new ClassicPawn
            {
                Color = configuration.Get("color", Convert.ToInt32),
                Location = configuration.Get("location", Convert.ToInt32),
                PlayerID = configuration.Get("playerid", Convert.ToInt32),
                Id = NextId(),
            };
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEntity CreateEel(XElement configuration)
        {
            try
            {

            return new ClassicEel
            {
                Top_location = configuration.Get("toplocation", Convert.ToInt32),
                Bottom_location = configuration.Get("bottomlocation", Convert.ToInt32),
                Id = NextId(),
            };
            }
            catch
            {
                throw new Exception();
            }
        }

        public IEntity CreateEscalator(XElement configuration)
        {
            try
            {

            return new ClassicEscalator
            {
                Top_location = configuration.Get("toplocation", Convert.ToInt32),
                Bottom_location = configuration.Get("bottomlocation", Convert.ToInt32),
                Id = NextId(),
            };
            }
            catch
            {
                throw new Exception();
            }
        }

        public void RollDice()
        {
            try
            {
            Random rnd = new Random();
            DiceResult = rnd.Next(1, DiceSides+1);
            }
            catch
            {
                throw new Exception();
            }

        }

        #region Private methods

        private long NextId()
        {
            _idCounter++;
            return _idCounter;
        }

        #endregion
    }
}
