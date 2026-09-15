class_name PlayerIdleState extends PlayerState


@export var deceleratoin: float = 8


func init() -> void:
	pass


func enter() -> void:
	print("Entered Idle State")
	player.animation_player.play("idle")


func exit() -> void:
	print("Exited Idle State")

func handle_input(_event: InputEvent) -> PlayerState:
	if _event.is_action_pressed("jump"):
		return jump

	return null


func process(_delta: float) -> PlayerState:
	if direction.x != 0:
		return run

	return null


func physics_process(_delta: float) -> PlayerState:
	player.update_velocity(0, deceleratoin)

	if not player.is_on_floor():
		return fall
	
	return null
