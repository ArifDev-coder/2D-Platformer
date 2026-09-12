class_name PlayerCrouchState extends PlayerState


func init() -> void:
	pass


func enter() -> void:
	print("Entered Crouch State")


func exit() -> void:
	print("Exited Crouch State")

func handle_input(_event: InputEvent) -> PlayerState:
	return null


func process(_delta: float) -> PlayerState:
	return null


func physics_process(_delta: float) -> PlayerState:
	return null