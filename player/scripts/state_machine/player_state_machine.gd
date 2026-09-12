@icon("res://player/sprites/icon_state_machine_16x16.png")
class_name PlayerStateMachine extends Node2D


@export var state_size: int = 3

var states: Array[PlayerState]
var current_state: PlayerState:
	get: return states.front()
var previous_state: PlayerState:
	get: return states[1]

var player: Player

func _ready() -> void:
	process_mode = Node.PROCESS_MODE_DISABLED


func _process(_delta: float) -> void:
	current_state.direction = Vector2(
		# sign(Input.get_axis("left", "right")),
		Input.get_axis("left", "right"),
		Input.get_axis("up", "down")
	)

	var new_state = current_state.process(_delta)
	change_state(new_state)


func _physics_process(_delta: float) -> void:
	var new_state = current_state.physics_process(_delta)
	change_state(new_state)


func _unhandled_input(_event: InputEvent) -> void:
	var new_state = current_state.handle_input(_event)
	change_state(new_state)


func change_state(new_state: PlayerState) -> void:
	if new_state == null:
		return
	elif new_state == current_state:
		return
	
	if current_state:
		current_state.exit()

	states.push_front(new_state)

	current_state.enter()

	states.resize(state_size)

	print(states)


func init(_player: Player) -> void:
	player = _player
	states = []

	for child in get_children():
		if child is PlayerState:
			states.append(child)

	print("Init found ", states.size(), " states")

	if states.size() == 0:
		return

	current_state.player = player
	current_state.state_machine = self

	for state in states:
		state.init()

	# change_state(current_state)
	change_state(current_state.crouch)
	change_state(previous_state.idle)

	process_mode = Node.PROCESS_MODE_INHERIT