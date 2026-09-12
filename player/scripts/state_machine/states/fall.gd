class_name PlayerFallState extends PlayerState


func init() -> void:
	pass


func enter() -> void:
	print("Entered Fall State")
	player.animation_player.play("fall")


func exit() -> void:
	print("Exited Fall State")

func handle_input(_event: InputEvent) -> PlayerState:
	return null


func process(_delta: float) -> PlayerState:
	return null


func physics_process(_delta: float) -> PlayerState:
	if player.is_on_floor():
		return idle

	return null