var audioSource : AudioSource;
private var originalVolume : float;

function Awake()
{
	originalVolume = audioSource.volume;
	//GameSounds.backgroundMusic = audioSource;
}