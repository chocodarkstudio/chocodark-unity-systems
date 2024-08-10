# GlobalAudio
`using ChocoDark.GlobalAudio;`

Controla globalmente todos los *AudioSource* registrados por el componente [*GlobalAudioSource*](./GlobalAudioSource.md).

Contiene funciones para reproducir clips, subir/bajar el volumen y pausar/des-pausar los *AudioSources*.


## Filtro por Canal
Los *AudioSources* pueden ser filtrados por canales, así especificar que *AudioSources* controlar.

Además cada canal contiene su propio volumen, que lo establecerá a los *AudioSources* asociados.

Canales:
- **None**: Ignora todas las llamadas de *GlobalAudio.*
- **All**: Recibe todas las llamadas de *GlobalAudio.*
- **Music**: Recibe únicamente las llamadas del canal ‘Music’ de *GlobalAudio.*
- **SFX**: Recibe únicamente las llamadas del canal ‘SFX’ de *GlobalAudio.*

> Puedes agregar más canales al Enum `Channel` modificando el archivo `Channels.cs`.

## ClipsPreset
Pendiente…

## Audio Mixer

*GlobalAudio* contiene funciones estáticas que facilitan establecer valores a los *Parámetros Expuestos* del Audio Mixer de Unity.

Para utilizarlas hay que asignar un Grupo en la propiedad *masterMixer*, en el inspector del componente *GlobalAudio.*


### Referencias
- [GlobalAudioSource](./GlobalAudioSource.md)