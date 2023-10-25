<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('wallets', function (Blueprint $table) {
            $table->id();
            $table->string(column:"address")->nullable();
            $table->string(column:"login")->nullable();
            $table->string(column:"private_key")->nullable();
            $table->string(column:"connection_one")->nullable();
            $table->string(column:"connection_two")->nullable();
            $table->string(column:"connection_three")->nullable();
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('wallets');
    }
};
