class_name PlayerRunState extends PlayerState


@export_custom(PROPERTY_HINT_NONE, "suffix:px/s") var speed: float = 100.0
@export_custom(PROPERTY_HINT_NONE, "suffix:px/s") var sprint_speed: float = 150.0
@export var acceleration: float = 4.0
@export var skid_acceleration: float = 8.0
@export var skid_audio: AudioStream

var current_acceleration: float
var current_direction: float = 0.0
var target_speed: float


func enter() -> void:
	print("Entered Run State")

	player.animation_player.play("run")

	current_acceleration = acceleration
	target_speed = speed

	if Input.is_action_pressed("action"):
		target_speed = sprint_speed

	player.animation_player.current_animation_changed.connect(_on_animation_changed)


func exit() -> void:
	print("Exited Run State")

	player.animation_player.current_animation_changed.disconnect(_on_animation_changed)
	player.audio_stream_player.stop()

	player.animation_player.speed_scale = 1



func handle_input(_event: InputEvent) -> PlayerState:
	if _event.is_action_pressed("action"):
		target_speed = sprint_speed
	elif _event.is_action_released("action"):
		target_speed = speed
	elif _event.is_action_pressed("jump"):
		return jump

	return null


func process(_delta: float) -> PlayerState:
	return null


func physics_process(_delta: float) -> PlayerState:
	if not player.is_on_floor():
		return fall

	if direction.x == 0:
		return idle
	elif sign(direction.x) == sign(player.velocity.x) or player.velocity.x == 0:
		current_acceleration = acceleration
		player.animation_player.play("run")

		player.animation_player.speed_scale = abs(player.velocity.x) / speed
	else:
		# Skidding
		current_acceleration = skid_acceleration
		player.animation_player.play("skid")

	if direction.x != current_direction:
		current_direction = direction.x
		player.update_direction(current_direction)

	player.update_velocity(direction.x * target_speed, current_acceleration)

	return null


func _on_animation_changed(_anim_name: String) -> void:
	if _anim_name == "skid":
		player.play_audio(skid_audio)
	else:
		player.audio_stream_player.stop()