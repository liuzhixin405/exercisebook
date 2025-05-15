extends Area2D

@export var bullet_speed : float = 100

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	await  get_tree().create_timer(3).timeout # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta: float) -> void:
	position +=Vector2(bullet_speed,0) * delta
	
