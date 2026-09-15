class_name PlayerJumpState extends PlayerState


@export var jump_velocity: float = 350.0
@export var jump_audio: AudioStream
@export_custom(PROPERTY_HINT_NONE, "suffix:px/s") var base_movement_speed: float = 100.0
@export var acceleration: float = 8.0

var move_speed: float

# func init() -> void:
	# pass


func enter() -> void:
	print("Entered Jump State")

	player.animation_player.play("jump")

	player.global_position.y -= 1
	
	move_speed = maxf(base_movement_speed, abs(player.velocity.x))

	player.velocity.y = - jump_velocity

	player.play_audio(jump_audio)


func exit() -> void:
	print("Exited Jump State")


func handle_input(_event: InputEvent) -> PlayerState:
	if _event.is_action_released("jump"):
		player.velocity.y *= 0.5 # Half of value
		return fall

	return null


func process(_delta: float) -> PlayerState:
	return null


func physics_process(_delta: float) -> PlayerState:
	player.update_velocity(direction.x * move_speed, acceleration)

	if player.is_on_floor():
		return idle
	elif player.velocity.y >= 0:
		return fall

	return null