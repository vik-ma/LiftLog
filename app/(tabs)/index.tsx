import React, { useState } from "react";
import Button from "../../components/Button";
import { Text, View } from "../../components/Themed";

export default function TabOneScreen() {
  return (
    <View className="flex justify-center items-center h-full">
      <View className="flex justify-center items-center">
        <Text className="text-2xl font-bold pb-3">Profiles</Text>
        <Button
          label="Create Profile"
          styling="bg-green-500 mb-4"
          onPress={() => {}}
        />
        <Button
          label="Delete Profile"
          styling="bg-red-500"
          onPress={() => {}}
        />
      </View>
    </View>
  );
}
