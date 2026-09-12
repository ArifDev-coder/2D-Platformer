class_name PlayerJumpState extends PlayerState


func init() -> void:
	pass


func enter() -> void:
	print("Entered Jump State")


func exit() -> void:
	print("Exited Jump State")

func handle_input(_event: InputEvent) -> PlayerState:
	return null


func process(_delta: float) -> PlayerState:
	return null


func physics_process(_delta: float) -> PlayerState:
	return null