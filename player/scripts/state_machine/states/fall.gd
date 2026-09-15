class_name PlayerFallState extends PlayerState


@export_custom(PROPERTY_HINT_NONE, "suffix:px/s") var base_movement_speed: float = 100.0
@export var fall_gravity_multiplier: float = 1.165 # Nint
@export var acceleration: float = 8.0
@export var coyote_time: float = 0.125

var coyote_timer: float
var move_speed: float


# func init() -> void:
	# pass


func enter() -> void:
	print("Entered Fall State")
	player.animation_player.play("fall")

	move_speed = maxf(base_movement_speed, abs(player.velocity.x))
	coyote_timer = coyote_time
	player.gravity_multiplier = fall_gravity_multiplier

	if state_machine.previous_state == jump:
		coyote_timer = 0


func exit() -> void:
	print("Exited Fall State")
	player.gravity_multiplier = 1.0

func handle_input(_event: InputEvent) -> PlayerState:
	if coyote_timer > 0:
		if _event.is_action_pressed("jump"):
			return jump
	
	return null


func process(_delta: float) -> PlayerState:
	return null


func physics_process(_delta: float) -> PlayerState:
	coyote_timer -= _delta

	player.update_velocity(direction.x * move_speed, acceleration)

	if player.is_on_floor():
		return idle

	return null