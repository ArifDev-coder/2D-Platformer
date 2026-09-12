class_name Player extends CharacterBody2D


@export var move_speed: float = 100.0

@onready var sprite: Sprite2D = $Sprite2D
@onready var animation_player: AnimationPlayer = $Sprite2D/AnimationPlayer
@onready var audio_stream_player: AudioStreamPlayer2D = $AudioStreamPlayer2D
@onready var player_state_machine: PlayerStateMachine = $PlayerStateMachine

func _ready() -> void:
	player_state_machine.init(self)

func _physics_process(delta: float) -> void:
	if not is_on_floor():
		velocity += get_gravity() * delta

	move_and_slide()


func update_velocity(_velocity: float, _acceleration: float) -> void:
	velocity.x = move_toward(velocity.x, _velocity, _acceleration)


func play_audio(audio: AudioStream) -> void:
	if audio == null: 
		return

	audio_stream_player.stream = audio
	audio_stream_player.play()

func update_direction(dir: float) -> void:
	if dir < 0:
		sprite.scale.x = -1
	else:
		sprite.scale.x = 1