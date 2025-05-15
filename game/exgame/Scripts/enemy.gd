extends Area2D

@export var slime_speed : float = -100

var is_dead : bool = false
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _physics_process(delta: float) -> void:
	if not is_dead:
		position += Vector2(slime_speed,0) * delta
	if position.x < -250:
		queue_free()


func _on_body_entered(body: Node2D) -> void:
	if body is CharacterBody2D and not is_dead:
		body.game_over()


func _on_area_entered(area: Area2D) -> void:
	if area.is_in_group("bullet"):
		$AnimatedSprite2D.play("death")
		is_dead = true
		area.queue_free()
		get_tree().current_scene.score +=1
		
		$DeathSound.play()
		# 怪物被击中后删除遗体
		await  get_tree().create_timer(0.6).timeout
		queue_free()
