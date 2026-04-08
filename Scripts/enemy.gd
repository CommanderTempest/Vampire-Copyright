extends CharacterBody2D

@export var speed = 120.0
@export var damage = 1

var player = null
var can_damage = true

func _ready():
	player = get_parent().get_node("Player")

func _physics_process(delta):
	if player:
		var direction = (player.global_position - global_position).normalized()
		velocity = direction * speed
		move_and_slide()

func _on_hitbox_body_entered(body):
	if body.has_method("take_damage") and can_damage:
		can_damage = false
		
		var timer = get_tree().create_timer(1.0)
		
		body.take_damage(damage)
		
		await timer.timeout
		
		can_damage = true
