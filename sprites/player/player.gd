extends CharacterBody2D

@export var speed = 200.0
@export var max_hp = 3
@export var attack_distance = 40.0

var hp = 3
var attacking = false

@onready var attack_pivot = $AttackPivot
@onready var attack_area = $AttackPivot/AttackArea
@onready var slash_sprite = $AttackPivot/SlashSprite
@onready var attack_timer = $Timer

func _ready():
	hp = max_hp
	attack_area.monitoring = false
	attack_area.position = Vector2.ZERO
	slash_sprite.visible = false
	slash_sprite.position = Vector2.ZERO

func _physics_process(delta):
	var direction = Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")
	
	if not attacking:
		velocity = direction * speed
	else:
		velocity = Vector2.ZERO
		
	move_and_slide()

func _process(delta):
	if Input.is_action_just_pressed("ui_accept") and not attacking:
		attack()

func attack():
	attacking = true

	var mouse_direction = (get_global_mouse_position() - global_position).normalized()

	if mouse_direction == Vector2.ZERO:
		mouse_direction = Vector2.RIGHT

	# 🔥 ONLY rotate pivot now
	attack_pivot.rotation = mouse_direction.angle()

	attack_area.monitoring = true
	slash_sprite.visible = true

	attack_timer.start()
	await attack_timer.timeout

	attack_area.monitoring = false
	slash_sprite.visible = false
	attacking = false

func take_damage(amount):
	hp -= amount
	print("Player HP:", hp)
	
	if hp <= 0:
		die()

func die():
	get_parent().show_game_over()
	queue_free()

func _on_attack_area_body_entered(body):
	if body.name == "Enemy":
		get_parent().add_score(1)
		body.queue_free()
