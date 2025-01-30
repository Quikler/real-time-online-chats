import Button from "@src/components/ui/Button";
import Input from "@src/components/ui/Input";
import Modal from "@src/components/ui/Modal";
import React, { useState } from "react";

export interface CreateChatFormData {
  title: string;
}

type Props = {
  onSubmit: (e: React.FormEvent<HTMLFormElement>, data: CreateChatFormData) => void;
  isChatFormOpen: boolean;
  setIsChatFormOpen: React.Dispatch<React.SetStateAction<boolean>>;
};

const CreateChatForm = ({ onSubmit, isChatFormOpen, setIsChatFormOpen }: Props) => {
  const [formData, setFormData] = useState<CreateChatFormData>({
    title: "",
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;

    setFormData((prevData) => ({
      ...prevData,
      [name]: value,
    }));
  };

  const handleCreateChatFormSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    onSubmit(e, formData);
  };

  return (
    <Modal isModalOpen={isChatFormOpen} setIsModalOpen={setIsChatFormOpen} title={"Create chat"}>
      <form className="space-y-4" onSubmit={handleCreateChatFormSubmit}>
        <div>
          <label className="block mb-2 text-sm font-medium text-white">Title</label>
          <Input
            onChange={handleChange}
            value={formData.title}
            type="text"
            name="title"
            id="title"
            className="w-full"
            placeholder="Enter title"
          />
        </div>
        <Button type="submit" className="w-full">
          Create chat
        </Button>
      </form>
    </Modal>
  );
};

export default CreateChatForm;
