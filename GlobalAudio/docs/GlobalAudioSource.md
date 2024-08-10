# GlobalAudioSource
`using ChocoDark.GlobalAudio;`

Registra un *AudioSource* para ser controlado mediante las llamadas de la clase [GlobalAudio](./GlobalAudio.md).

Modos de reproducción (*PlayMode*):
- **Interrupt**: Detiene los clips que ya se estén reproduciendo en el *AudioSource*, luego reproduce el nuevo clip.
- **Additive**: Reproduce el clip simultáneamente a otros clips que ya se estén reproduciendo en el *AudioSource*.
- **Ignore**: Ignora las llamadas de reproducción y pausa que se hagan al *GlobalAudioSource*.

## Control por script
Funciones para controlar *GlobalAudioSource* mediante script

### Play clip
```csharp
PlayClip(AudioClip clip, float volume = 1)
```

Reproduce un clip, siempre que el modo de reproducción no sea *ignore*.

### Pause
```csharp
Pause(bool pause)
```

Pausa la reproducción del *AudioSource*.

El modo de reproducción *ignore* solo se tiene en cuenta si el canal esta configurado en *All*. De lo contrario, siempre hará caso a la llamada.

### Set Volume
```csharp
SetVolume(float volume)
```

Establece el volumen del *AudioSource*.

### Referencias
- [GlobalAudio](./GlobalAudio.md)