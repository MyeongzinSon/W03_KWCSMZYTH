using UnityEngine;

namespace Minigames.Mario.Scripts {
    public class movementLimiter : MonoBehaviour {
        public static movementLimiter instance;

        [SerializeField] bool _initialCharacterCanMove = true;
        public bool CharacterCanMove;

        private void OnEnable() {
            instance = this;
        }

        private void Start() {
            CharacterCanMove = _initialCharacterCanMove;
        }
    }
}